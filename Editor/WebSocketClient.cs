using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sandbox;
using ModelContextProtocol.Commands;
using Editor;

namespace ModelContextProtocol;

public static class WebSocketClient
{
	private static WebSocket _webSocket;
	private static CommandDispatcher _commandDispatcher = new();
	private static readonly CancellationTokenSource _cancellationTokenSource = new();

	public static async Task Connect()
	{
		if ( _webSocket != null ) {
			// Dispose the old WebSocket if it exists
			_webSocket.Dispose();
		}

		_webSocket = new();

		Log.Info( "Connecting to MCP Server..." );

		try
		{
			await _webSocket.Connect( "ws://localhost:8080/ws" );
			_webSocket.OnMessageReceived += OnMessageReceived;
			
			EditorUtility.PlayRawSound( "sounds/editor/success.wav" );

			Log.Info( "Connected to MCP Server" );
		}
		catch ( Exception e )
		{
			EditorUtility.PlayRawSound( "sounds/editor/fail.wav" );

			Log.Error( "Failed to connect to MCP Server: " + e.Message );
		}
	}
	
	public static WebSocket.MessageReceivedHandler OnMessageReceived { get; private set; } = ( message ) =>
	{
		Log.Info( "Received message from MCP Server: " + message );
		
		// Process the command asynchronously without blocking
		// Use ConfigureAwait(false) and handle exceptions properly
		var task = Task.Run( async () =>
		{
			// Check cancellation token at the start
			_cancellationTokenSource.Token.ThrowIfCancellationRequested();
			
			try
			{
				var response = await _commandDispatcher.ProcessCommandAsync( message ).ConfigureAwait(false);
				
				// Check cancellation and disposal before sending
				_cancellationTokenSource.Token.ThrowIfCancellationRequested();

				// Send response back to MCP server
				Send( response );
			}
			catch (OperationCanceledException)
			{
				// Expected when cancellation is requested, don't log as error
				Log.Info("Command processing was cancelled");
			}
			catch ( Exception ex )
			{
				Log.Error( $"Error processing command: {ex.Message}" );

				// Send error response back
				try
				{
					var errorResponse = System.Text.Json.JsonSerializer.Serialize( new
					{
						success = false,
						error = ex.Message
					} );
					
					Send( errorResponse );
				}
				catch (Exception sendEx)
				{
					Log.Error( $"Failed to send error response: {sendEx.Message}" );
				}
			}
		}, _cancellationTokenSource.Token );

		// Handle task completion and exceptions properly
		task.ContinueWith( completedTask =>
		{
			if ( completedTask.IsFaulted && !_cancellationTokenSource.Token.IsCancellationRequested )
			{
				var exception = completedTask.Exception?.GetBaseException();
				if (exception is not OperationCanceledException)
				{
					Log.Error( $"Unhandled exception in command processing: {exception?.Message}" );
				}
			}
		}, TaskContinuationOptions.ExecuteSynchronously );
	};

	public static void Send( string message )
	{
		try
		{
			if ( _webSocket.IsConnected )
			{
				_webSocket.Send( message );
				Log.Info( "MCP message sent: " + message );
			}
			else
			{
				Log.Warning( "Cannot send message - Not connected to MCP Server" );
			}
		}
		catch (ObjectDisposedException)
		{
			Log.Warning("Cannot send message - Connection to MCP Server has been disposed");
		}
		catch (Exception ex)
		{
			Log.Error($"Error sending message: {ex.Message}");
		}
	}

	public static void Dispose()
	{
		try
		{
			Log.Info("Disposing MCP Server connection...");
			
			// Cancel all pending operations first
			_cancellationTokenSource.Cancel();
			
			// Dispose the WebSocket
			_webSocket?.Dispose();
			
			// Dispose the cancellation token source
			_cancellationTokenSource?.Dispose();
			
			Log.Info("MCP Server connection disposed successfully");
		}
		catch (Exception ex)
		{
			Log.Error($"Error during disposal: {ex.Message}");
		}
	}
}
