using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SandboxModelContextProtocol.Server.Services.Interfaces;
using SandboxModelContextProtocol.Server.Services.Models;

namespace SandboxModelContextProtocol.Server.Services;

public class EditorToolService( ILogger<EditorToolService> logger, IServiceProvider serviceProvider ) : IEditorToolService
{
	private readonly ILogger<EditorToolService> _logger = logger;
	private readonly ConcurrentDictionary<string, TaskCompletionSource<CallEditorToolResponse>> _pendingCommands = new();
	private readonly IWebSocketService _webSocketService = serviceProvider.GetRequiredService<IWebSocketService>();

	public async Task<CallEditorToolResponse> CallTool( CallEditorToolRequest request )
	{
		// Generate unique command ID
		var id = Guid.NewGuid().ToString();

		_logger.LogInformation( "Executing tool call: {Name}", request.Name );

		// Find active connections
		var activeConnections = _webSocketService.GetWebSocketConnections()
			.Where( conn => conn.IsConnected )
			.ToList();

		// If no active connections, return an error
		if ( activeConnections.Count == 0 )
		{
			return new CallEditorToolResponse()
			{
				Id = id,
				Name = request.Name,
				Content = [JsonSerializer.SerializeToElement( "No active s&box connections available" )],
				IsError = true
			};
		}

		// Add command ID to request
		request.Id = id;
		var commandJson = JsonSerializer.Serialize( request );

		// Create task completion source for this command
		var tcs = new TaskCompletionSource<CallEditorToolResponse>();
		_pendingCommands[id] = tcs;

		try
		{
			// Send command to all active s&box connections
			await _webSocketService.SendToAll( commandJson );

			_logger.LogInformation( "Tool call sent to s&box connections" );

			// Wait for response with timeout
			using var cts = new CancellationTokenSource( TimeSpan.FromSeconds( 30 ) );
			cts.Token.Register( () =>
			{
				if ( tcs.TrySetCanceled() )
				{
					_pendingCommands.TryRemove( id, out _ );
					_logger.LogWarning( "Tool call {Id} timed out", id );
				}
			} );

			return await tcs.Task;
		}
		catch ( OperationCanceledException )
		{
			_pendingCommands.TryRemove( id, out _ );
			return new CallEditorToolResponse()
			{
				Id = id,
				Name = request.Name,
				Content = [JsonSerializer.SerializeToElement( "Tool call timed out after 30 seconds" )],
				IsError = true
			};
		}
		catch ( Exception ex )
		{
			_pendingCommands.TryRemove( id, out _ );
			_logger.LogError( ex, "Failed to send tool call to s&box connections" );
			return new CallEditorToolResponse()
			{
				Id = id,
				Name = request.Name,
				Content = [JsonSerializer.SerializeToElement( $"Failed to send tool call: {ex.Message}" )],
				IsError = true
			};
		}
	}

	public void HandleResponse( string message )
	{
		_logger.LogInformation( "Handling response: {Message}", message );

		CallEditorToolResponse? response = JsonSerializer.Deserialize<CallEditorToolResponse>( message );
		if ( response == null )
		{
			_logger.LogWarning( "Failed to parse response JSON: {Message}", message );
			return;
		}

		if ( _pendingCommands.TryRemove( response.Id, out var tcs ) )
		{
			tcs.SetResult( response );
			_logger.LogInformation( "Tool call {Id} completed successfully", response.Id );
		}
	}
}
