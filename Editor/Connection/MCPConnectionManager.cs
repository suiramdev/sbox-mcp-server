using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Editor;
using Sandbox;
using SandboxModelContextProtocol.Editor.Connection.Models;
using SandboxModelContextProtocol.Editor.Tools;
using SandboxModelContextProtocol.Editor.Tools.Models;

namespace SandboxModelContextProtocol.Editor.Connection;

/// <summary>
/// Manages MCP WebSocket connections with status tracking and event emission
/// </summary>
public static class McpConnectionManager
{
	private static WebSocket? _webSocket;
	private static CancellationTokenSource? _cancellationTokenSource;

	/// <summary>
	/// Current connection status and diagnostics
	/// </summary>
	public static McpConnectionStatus Status { get; private set; } = new();

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
				Log.Error( "Cannot send response - Not connected to MCP Server" );
			}
		}
		catch ( ObjectDisposedException )
		{
			Log.Error( "Cannot send response - Connection to MCP Server has been disposed" );
		}
		catch ( Exception ex )
		{
			Log.Error( $"Error sending response: {ex.Message}" );
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
		_ = Task.Run( async () =>
		{
			// Check cancellation token at the start
			_cancellationTokenSource?.Token.ThrowIfCancellationRequested();

			try
			{
				CallEditorToolRequest? request = JsonSerializer.Deserialize<CallEditorToolRequest>( message );
				if ( request == null )
				{
					return;
				}

				CallEditorToolResponse? response = await McpToolExecutor.CallEditorTool( request );
				await Send( JsonSerializer.Serialize( response ) );
			}
			catch ( Exception ex )
			{
				Log.Error( $"Error calling tool: {ex.Message}\n{ex.StackTrace}" );
			}
		}, _cancellationTokenSource?.Token ?? CancellationToken.None );
	};
}
