using System.Collections.Generic;
using System.Threading.Tasks;
using SandboxModelContextProtocol.Server.Services.Models;

namespace SandboxModelContextProtocol.Server.Services.Interfaces;

public interface IWebSocketService
{
	/// <summary>
	/// Get all connected WebSocket connections
	/// </summary>
	/// <returns>An enumerable of WebSocket connections</returns>
	IEnumerable<WebSocketConnection> GetWebSocketConnections();

	/// <summary>
	/// Get a connection by its ID
	/// </summary>
	/// <param name="connectionId">The ID of the connection</param>
	/// <returns>The connection</returns>
	WebSocketConnection? GetWebSocketConnection( string connectionId );

	/// <summary>
	/// Send a message to all connected clients
	/// </summary>
	/// <param name="message">The message to send</param>
	/// <returns>A task</returns>
	Task SendToAll( string message );
}
