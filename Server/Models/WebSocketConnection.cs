using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SandboxModelContextProtocol.Server.Models;

public class WebSocketConnection( WebSocket webSocket, ILogger logger )
{
	private readonly WebSocket _webSocket = webSocket;
	private readonly ILogger _logger = logger;

	public bool IsConnected => _webSocket.State == WebSocketState.Open;

	/// <summary>
	/// Send a message to the WebSocket connection
	/// </summary>
	/// <param name="message">The message to send</param>
	/// <returns>A task</returns>
	public async Task SendAsync( string message )
	{
		if ( !IsConnected )
		{
			_logger.LogWarning( "Attempted to send message to disconnected WebSocket" );
			return;
		}

		try
		{
			var bytes = Encoding.UTF8.GetBytes( message );
			await _webSocket.SendAsync( new ArraySegment<byte>( bytes ), WebSocketMessageType.Text, true, CancellationToken.None );
			_logger.LogDebug( "Message sent to WebSocket: {Message}", message );
		}
		catch ( WebSocketException ex )
		{
			_logger.LogError( ex, "Failed to send WebSocket message: {Message}", message );
			throw;
		}
		catch ( Exception ex )
		{
			_logger.LogError( ex, "Unexpected error sending WebSocket message: {Message}", message );
			throw;
		}
	}
}