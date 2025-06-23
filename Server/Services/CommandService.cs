using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SandboxModelContextProtocol.Server.Models;
using SandboxModelContextProtocol.Server.Services.Interfaces;

namespace SandboxModelContextProtocol.Server.Services;

public class CommandService( ILogger<CommandService> logger, IServiceProvider serviceProvider ) : ICommandService
{
	private readonly ILogger<CommandService> _logger = logger;
	private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _pendingCommands = new();
	private readonly IWebSocketService _webSocketService = serviceProvider.GetRequiredService<IWebSocketService>();

	public async Task<CommandResponse> ExecuteCommandAsync( CommandRequest request )
	{
		// Generate unique command ID
		var commandId = Guid.NewGuid().ToString();

		_logger.LogInformation( "Executing command: {Command}", request.Command );

		// Find active connections
		var activeConnections = _webSocketService.GetWebSocketConnections()
			.Where( conn => conn.IsConnected )
			.ToList();

		// If no active connections, return an error
		if ( activeConnections.Count == 0 )
		{
			return new CommandResponse()
			{
				CommandId = commandId,
				Content = "No active s&box connections available",
				IsError = true
			};
		}

		// Add command ID to request
		request.CommandId = commandId;
		var commandJson = JsonSerializer.Serialize( request );

		// Create task completion source for this command
		var tcs = new TaskCompletionSource<string>();
		_pendingCommands[commandId] = tcs;

		try
		{
			// Send command to all active s&box connections
			await _webSocketService.SendToAll( commandJson );

			_logger.LogInformation( "Command sent to s&box connections" );

			// Wait for response with timeout
			using var cts = new CancellationTokenSource( TimeSpan.FromSeconds( 30 ) );
			cts.Token.Register( () =>
			{
				if ( tcs.TrySetCanceled() )
				{
					_pendingCommands.TryRemove( commandId, out _ );
					_logger.LogWarning( "Command {CommandId} timed out", commandId );
				}
			} );

			return new CommandResponse()
			{
				CommandId = commandId,
				Content = await tcs.Task,
				IsError = false
			};
		}
		catch ( OperationCanceledException )
		{
			_pendingCommands.TryRemove( commandId, out _ );
			return new CommandResponse()
			{
				CommandId = commandId,
				Content = "Command timed out after 30 seconds",
				IsError = true
			};
		}
		catch ( Exception ex )
		{
			_pendingCommands.TryRemove( commandId, out _ );
			_logger.LogError( ex, "Failed to send command to s&box connections" );
			return new CommandResponse()
			{
				CommandId = commandId,
				Content = $"Failed to send command: {ex.Message}",
				IsError = true
			};
		}
	}

	public void HandleResponse( string message )
	{
		_logger.LogInformation( "Handling response: {Message}", message );

		CommandResponse? response = JsonSerializer.Deserialize<CommandResponse>( message );
		if ( response == null )
		{
			_logger.LogWarning( "Failed to parse response JSON: {Message}", message );
			return;
		}

		if ( _pendingCommands.TryRemove( response.CommandId, out var tcs ) )
		{
			tcs.SetResult( response.Content );
			_logger.LogInformation( "Command {CommandId} completed successfully", response.CommandId );
		}
	}
}
