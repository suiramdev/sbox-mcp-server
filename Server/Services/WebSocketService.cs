using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using SandboxModelContextProtocol.Server.Services.Interfaces;
using SandboxModelContextProtocol.Server.Services.Models;
using System.IO;

namespace SandboxModelContextProtocol.Server.Services;

public class WebSocketService( ILogger<WebSocketService> logger, IConfiguration configuration, IOptions<Models.WebSocketOptions> options, IServiceProvider serviceProvider ) : IHostedService, IWebSocketService
{
	private readonly ILogger<WebSocketService> _logger = logger;
	private readonly IConfiguration _configuration = configuration;
	private readonly Models.WebSocketOptions _options = options.Value;
	private readonly IServiceProvider _serviceProvider = serviceProvider;
	private WebApplication? _app;
	private readonly ConcurrentDictionary<WebSocketConnection, string> _connections = new();

	/// <summary>
	/// Start the WebSocket server
	/// </summary>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>A task</returns>
	public async Task StartAsync( CancellationToken cancellationToken )
	{
		var builder = WebApplication.CreateBuilder();

		// Configure the application with the same configuration
		builder.Configuration.AddConfiguration( _configuration );

		// Configure the URL from configuration
		builder.WebHost.UseUrls( _options.Url );

		_app = builder.Build();

		_app.UseWebSockets();

		_app.Map( _options.Path, async context =>
		{
			if ( context.WebSockets.IsWebSocketRequest )
			{
				using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
				await HandleWebSocketConnection( webSocket, cancellationToken );
			}
			else
			{
				context.Response.StatusCode = 400;
			}
		} );

		await _app.StartAsync( cancellationToken );
		_logger.LogInformation( "WebSocket Server started on {WebSocketUrl}{WebSocketPath}", _options.Url, _options.Path );
	}

	/// <summary>
	/// Get all connected WebSocket connections
	/// </summary>
	/// <returns>An enumerable of WebSocket connections</returns>
	public IEnumerable<WebSocketConnection> GetWebSocketConnections()
	{
		return _connections.Keys;
	}

	/// <summary>
	/// Get a connection by its ID
	/// </summary>
	/// <param name="connectionId">The ID of the connection</param>
	/// <returns>The connection</returns>
	public WebSocketConnection? GetWebSocketConnection( string connectionId )
	{
		return _connections.FirstOrDefault( kvp => kvp.Value == connectionId ).Key;
	}

	/// <summary>
	/// Send a message to all connected clients
	/// </summary>
	/// <param name="message">The message to send</param>
	/// <returns>A task</returns>
	public async Task SendToAll( string message )
	{
		foreach ( var connection in _connections.Keys )
		{
			await connection.SendAsync( message );
		}
	}

	/// <summary>
	/// Stop the WebSocket server
	/// </summary>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>A task</returns>
	public async Task StopAsync( CancellationToken cancellationToken )
	{
		if ( _app != null )
		{
			await _app.StopAsync( cancellationToken );
			_logger.LogInformation( "WebSocket Server stopped" );
		}
	}

	private void RegisterWebSocketConnection( WebSocketConnection connection )
	{
		var connectionId = Guid.NewGuid().ToString();
		_connections[connection] = connectionId;
		_logger.LogInformation( "WebSocket connection registered with ID: {ConnectionId}", connectionId );
	}

	private void UnregisterWebSocketConnection( WebSocketConnection connection )
	{
		_connections.TryRemove( connection, out var connectionId );
		_logger.LogInformation( "WebSocket connection unregistered: {ConnectionId}", connectionId );
	}

	private async Task HandleWebSocketConnection( WebSocket webSocket, CancellationToken cancellationToken )
	{
		var connection = new WebSocketConnection( webSocket, _logger );
		RegisterWebSocketConnection( connection );

		var buffer = new byte[1024 * 4];

		try
		{
			_logger.LogInformation( "WebSocket connection established" );

			while ( webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested )
			{
				using var ms = new MemoryStream();
				WebSocketReceiveResult result;

				do
				{
					result = await webSocket.ReceiveAsync( new ArraySegment<byte>( buffer ), cancellationToken );
					ms.Write( buffer, 0, result.Count );
				} while ( !result.EndOfMessage && !cancellationToken.IsCancellationRequested );

				if ( result.MessageType == WebSocketMessageType.Text )
				{
					var message = Encoding.UTF8.GetString( ms.ToArray() );
					_logger.LogInformation( "Received from s&box: {Message}", message );

					// Handle responses from s&box
					var commandService = _serviceProvider.GetRequiredService<IEditorToolService>();
					commandService.HandleResponse( message );
				}
				else if ( result.MessageType == WebSocketMessageType.Close )
				{
					_logger.LogInformation( "WebSocket close message received from client" );
					await webSocket.CloseAsync( WebSocketCloseStatus.NormalClosure, "Connection closed by client", cancellationToken );
					break;
				}
			}
		}
		catch ( OperationCanceledException )
		{
			_logger.LogInformation( "WebSocket connection cancelled" );
		}
		catch ( WebSocketException ex )
		{
			_logger.LogError( ex, "WebSocket error occurred" );
		}
		catch ( Exception ex )
		{
			_logger.LogError( ex, "Unexpected error in WebSocket connection" );
		}
		finally
		{
			UnregisterWebSocketConnection( connection );
			_logger.LogInformation( "WebSocket connection closed and unregistered" );
		}
	}
}

