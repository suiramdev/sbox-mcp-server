using System;
using System.Threading;
using System.Threading.Tasks;
using Sandbox;
using SandboxModelContextProtocol.Editor.Models;

namespace SandboxModelContextProtocol.Editor.Services;

public static class WebSocketService
{
	private static WebSocket _webSocket;
	private static CancellationTokenSource _cancellationTokenSource;

	public static async Task Connect()
	{
		// Clean up existing connection if any
		Disconnect();

		// Create new connection
		_webSocket = new WebSocket();
		_cancellationTokenSource = new CancellationTokenSource();

		await _webSocket.Connect( "ws://localhost:8080/ws" );
		_webSocket.OnMessageReceived += OnMessageReceived;
		_webSocket.OnDisconnected += OnDisconnected;
	}

	public static async Task Send( string message )
	{
		try
		{
			if ( _webSocket?.IsConnected == true )
			{
				await _webSocket.Send( message );
				Log.Info( $"Sent response to MCP Server: {message}" );
			}
			else
			{
				Log.Warning( "Cannot send response - Not connected to MCP Server" );
			}
		}
		catch ( ObjectDisposedException )
		{
			Log.Warning( "Cannot send response - Connection to MCP Server has been disposed" );
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
	}

	private static readonly WebSocket.DisconnectedHandler OnDisconnected = ( status, reason ) =>
	{
		Log.Warning( $"Disconnected from MCP Server: {reason} (Code: {status})" );
	};

	private static readonly WebSocket.MessageReceivedHandler OnMessageReceived = ( message ) =>
	{
		// Process the command asynchronously without blocking
		var task = Task.Run( async () =>
		{
			// Check cancellation token at the start
			_cancellationTokenSource?.Token.ThrowIfCancellationRequested();

			// Deserialize the command request
			CommandRequest request = default;
			try
			{
				request = System.Text.Json.JsonSerializer.Deserialize<CommandRequest>( message );
				Log.Info( $"Received command request: {request.CommandId}" );
			}
			catch ( Exception ex )
			{
				Log.Error( $"Error deserializing a incoming command request: {ex.Message}" );
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
				var response = await CommandService.Instance.ExecuteCommandAsync( request ).ConfigureAwait( false );

				// Check cancellation before sending
				_cancellationTokenSource?.Token.ThrowIfCancellationRequested();

				// Send response back to MCP server
				await Send( System.Text.Json.JsonSerializer.Serialize( response ) );
			}
			catch ( OperationCanceledException )
			{
				// Expected when cancellation is requested, don't log as error
				Log.Info( $"Command request {request.CommandId} was cancelled" );
			}
			catch ( Exception ex )
			{
				Log.Error( $"Error processing command request {request.CommandId}: {ex.Message}" );
			}
		}, _cancellationTokenSource?.Token ?? CancellationToken.None );
	};
}
