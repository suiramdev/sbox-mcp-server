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

namespace ModelContextProtocol.Server.Services;

public class WebSocketOptions
{
	public string Url { get; set; } = "http://localhost:8080";
	public string Path { get; set; } = "/ws";
}

public interface IWebSocketConnection
{
	bool IsConnected { get; }
	Task SendAsync( string message );
}

public class WebSocketConnection( WebSocket webSocket, ILogger logger ) : IWebSocketConnection
{
	private readonly WebSocket _webSocket = webSocket;
	private readonly ILogger _logger = logger;

	public bool IsConnected => _webSocket.State == WebSocketState.Open;

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

public class WebSocketService( ILogger<WebSocketService> logger, IConfiguration configuration, IOptions<WebSocketOptions> options, ICommandService commandService ) : IHostedService
{
	private readonly ILogger<WebSocketService> _logger = logger;
	private readonly IConfiguration _configuration = configuration;
	private readonly WebSocketOptions _options = options.Value;
	private readonly ICommandService _commandService = commandService;
	private WebApplication? _app;

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

	private async Task HandleWebSocketConnection( WebSocket webSocket, CancellationToken cancellationToken )
	{
		var connection = new WebSocketConnection( webSocket, _logger );
		_commandService.RegisterWebSocketConnection( connection );

		var buffer = new byte[1024 * 4];

		try
		{
			_logger.LogInformation( "WebSocket connection established" );

			while ( webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested )
			{
				var result = await webSocket.ReceiveAsync( new ArraySegment<byte>( buffer ), cancellationToken );

				if ( result.MessageType == WebSocketMessageType.Text )
				{
					var message = Encoding.UTF8.GetString( buffer, 0, result.Count );
					_logger.LogInformation( "Received from s&box: {Message}", message );

					// Handle responses from s&box
					_commandService.HandleResponse( message );
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
			_commandService.UnregisterWebSocketConnection( connection );
			_logger.LogInformation( "WebSocket connection closed and unregistered" );
		}
	}

	public async Task StopAsync( CancellationToken cancellationToken )
	{
		if ( _app != null )
		{
			await _app.StopAsync( cancellationToken );
			_logger.LogInformation( "WebSocket Server stopped" );
		}
	}
}

