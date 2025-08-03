using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SandboxModelContextProtocol.Server.Services;
using SandboxModelContextProtocol.Server.Services.Interfaces;
using SandboxModelContextProtocol.Server.Services.Models;
using Microsoft.Extensions.Configuration;

namespace SandboxModelContextProtocol.Server;

public class Program
{
	public static async Task Main( string[] args )
	{
		var builder = WebApplication.CreateBuilder( args );

		builder.WebHost.UseUrls( "http://0.0.0.0:8080" );

		// Configure logging for HTTP transport
		builder.Logging.AddConsole();

		// Configure WebSocket options
		builder.Services.Configure<Services.Models.WebSocketOptions>(
			builder.Configuration.GetSection( "WebSocket" )
		);

		// Register the command service
		builder.Services.AddSingleton<IToolService, ToolService>();

		// Register the resource service
		builder.Services.AddSingleton<IResourceService, ResourceService>();

		// Configure MCP Server with HTTP transport, tools and resources from assembly
		builder.Services
			.AddMcpServer()
			.WithHttpTransport()
			.WithToolsFromAssembly()
			.WithResourcesFromAssembly();

		// Register WebSocket service as singleton (no longer a hosted service)
		builder.Services.AddSingleton<WebSocketService>();
		builder.Services.AddSingleton<IWebSocketService>( provider => provider.GetRequiredService<WebSocketService>() );

		var app = builder.Build();

		// Enable WebSocket support
		app.UseWebSockets();

		// Map MCP endpoints for HTTP transport
		app.MapMcp();

		// Add health check endpoint for Docker
		app.MapGet( "/health", () => Results.Ok( new { status = "healthy", timestamp = DateTime.UtcNow } ) );

		// Configure WebSocket endpoint
		var webSocketService = app.Services.GetRequiredService<WebSocketService>();

		app.Map( "/ws", async context =>
		{
			if ( context.WebSockets.IsWebSocketRequest )
			{
				using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
				await webSocketService.HandleWebSocketConnection( webSocket, context.RequestAborted );
			}
			else
			{
				context.Response.StatusCode = 400;
			}
		} );

		await app.RunAsync();
	}
}
