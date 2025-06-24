using System;
using System.Threading;
using System.Threading.Tasks;
using Editor;
using Sandbox;
using SandboxModelContextProtocol.Editor.Commands.Models;
using SandboxModelContextProtocol.Editor.Commands;

namespace SandboxModelContextProtocol.Editor.Connection;

/// <summary>
/// Manages MCP WebSocket connections with status tracking and event emission
/// </summary>
public static class MCPConnectionManager
{
	private static WebSocket? _webSocket;
	private static CancellationTokenSource? _cancellationTokenSource;
	private static CommandManager _commandManager = new();

	/// <summary>
	/// Current connection status and diagnostics
	/// </summary>
	public static MCPConnectionStatus Status { get; private set; } = new();

	public static async Task Connect()
	{
		// Clean up existing connection if any
		Disconnect();

		// Reset status and emit connection started event
		Status.Reset();
		EditorEvent.Run( "mcp.connection.started" );

		try
		{
			// Create new connection
			_webSocket = new WebSocket();
			_cancellationTokenSource = new CancellationTokenSource();
			_commandManager = new CommandManager();

			await _webSocket.Connect( "ws://localhost:8080/ws" );
			_webSocket.OnMessageReceived += OnMessageReceived;
			_webSocket.OnDisconnected += OnDisconnected;

			// Update status and emit connection success event
			Status.SetConnected();
			EditorEvent.Run( "mcp.connection.success" );
		}
		catch ( Exception ex )
		{
			// Update status and emit connection failed event
			Status.SetFailed( ex.Message );
			EditorEvent.Run( "mcp.connection.failed" );
			throw;
		}
	}

	public static async Task Send( string message )
	{
		try
		{
			if ( _webSocket?.IsConnected == true )
			{
				await _webSocket.Send( message );
			}
			else
			{
				Status.AddDiagnostic( MCPConnectionDiagnostic.DiagnosticType.Warning, "Cannot send response - Not connected to MCP Server" );
			}
		}
		catch ( ObjectDisposedException )
		{
			Status.AddDiagnostic( MCPConnectionDiagnostic.DiagnosticType.Error, "Cannot send response - Connection to MCP Server has been disposed" );
		}
		catch ( Exception ex )
		{
			Status.AddDiagnostic( MCPConnectionDiagnostic.DiagnosticType.Error, $"Error sending response: {ex.Message}" );
		}
	}

	public static void Disconnect()
	{
		_cancellationTokenSource?.Cancel();
		_webSocket?.Dispose();
		_cancellationTokenSource?.Dispose();

		_webSocket = null;
		_cancellationTokenSource = null;

		var reason = "Manual disconnect";
		Status.SetDisconnected( reason );
		EditorEvent.Run( "mcp.disconnected" );
	}

	private static readonly WebSocket.DisconnectedHandler OnDisconnected = ( status, reason ) =>
	{
		var disconnectReason = $"{reason} (Code: {status})";
		Status.SetDisconnected( disconnectReason );
		EditorEvent.Run( "mcp.disconnected" );
	};

	private static readonly WebSocket.MessageReceivedHandler OnMessageReceived = ( message ) =>
	{
		// Process the command asynchronously without blocking
		var task = Task.Run( async () =>
		{
			// Check cancellation token at the start
			_cancellationTokenSource?.Token.ThrowIfCancellationRequested();

			// Deserialize the command request
			var request = CommandRequest.FromJson( message );
			if ( request == null )
			{
				Status.AddDiagnostic( MCPConnectionDiagnostic.DiagnosticType.Warning, $"Error deserializing a incoming command request: {message}" );
				await Send( System.Text.Json.JsonSerializer.Serialize( new CommandResponse()
				{
					CommandId = "error",
					Content = "Invalid command request",
					IsError = true
				} ) );
				return;
			}

			// Attempt to execute command
			try
			{
				var response = await _commandManager.HandleCommandAsync( request ).ConfigureAwait( false );

				// Check cancellation before sending
				_cancellationTokenSource?.Token.ThrowIfCancellationRequested();

				// Send response back to MCP server
				await Send( System.Text.Json.JsonSerializer.Serialize( response ) );
			}
			catch ( OperationCanceledException )
			{
				Status.AddDiagnostic( MCPConnectionDiagnostic.DiagnosticType.Info, $"Command request {request.CommandId} was cancelled" );
			}
			catch ( Exception ex )
			{
				Status.AddDiagnostic( MCPConnectionDiagnostic.DiagnosticType.Error, $"Error processing command request {request.CommandId}: {ex.Message}" );
			}
		}, _cancellationTokenSource?.Token ?? CancellationToken.None );
	};
}
