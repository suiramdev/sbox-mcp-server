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

public class ResourceService( ILogger<ResourceService> logger, IServiceProvider serviceProvider ) : IResourceService
{
	private readonly ILogger<ResourceService> _logger = logger;
	private readonly ConcurrentDictionary<string, TaskCompletionSource<CallResourceResponse>> _pendingRequests = new();
	private readonly IWebSocketService _webSocketService = serviceProvider.GetRequiredService<IWebSocketService>();

	public async Task<CallResourceResponse> GetResource( CallResourceRequest request )
	{
		_logger.LogInformation( "Getting resource: {Name}", request.Name );

		// Find active connections
		var activeConnections = _webSocketService.GetWebSocketConnections()
			.Where( conn => conn.IsConnected )
			.ToList();

		// If no active connections, return an error
		if ( activeConnections.Count == 0 )
		{
			return new CallResourceResponse()
			{
				Id = request.Id,
				Name = request.Name,
				Content = [JsonSerializer.SerializeToElement( "No active s&box connections available" )],
				IsError = true
			};
		}

		// Add command ID to request
		var requestJson = JsonSerializer.Serialize( request );

		// Create task completion source for this request
		var tcs = new TaskCompletionSource<CallResourceResponse>();
		_pendingRequests[request.Id] = tcs;

		try
		{
			// Send request to all active s&box connections
			await _webSocketService.SendToAll( requestJson );

			_logger.LogInformation( "Resource request sent to s&box connections" );

			// Wait for response with timeout
			using var cts = new CancellationTokenSource( TimeSpan.FromSeconds( 30 ) );
			cts.Token.Register( () =>
			{
				if ( tcs.TrySetCanceled() )
				{
					_pendingRequests.TryRemove( request.Id, out _ );
					_logger.LogWarning( "Resource request {Id} timed out", request.Id );
				}
			} );

			return await tcs.Task;
		}
		catch ( OperationCanceledException )
		{
			_pendingRequests.TryRemove( request.Id, out _ );
			return new CallResourceResponse()
			{
				Id = request.Id,
				Name = request.Name,
				Content = [JsonSerializer.SerializeToElement( "Resource request timed out after 30 seconds" )],
				IsError = true
			};
		}
		catch ( Exception ex )
		{
			_pendingRequests.TryRemove( request.Id, out _ );
			_logger.LogError( ex, "Failed to send resource request to s&box connections" );
			return new CallResourceResponse()
			{
				Id = request.Id,
				Name = request.Name,
				Content = [JsonSerializer.SerializeToElement( $"Failed to send resource request: {ex.Message}" )],
				IsError = true
			};
		}
	}

	public void HandleResponse( string message )
	{
		_logger.LogInformation( "Handling resource response: {Message}", message );

		CallResourceResponse? response = JsonSerializer.Deserialize<CallResourceResponse>( message );
		if ( response == null )
		{
			_logger.LogWarning( "Failed to parse resource response JSON: {Message}", message );
			return;
		}

		if ( _pendingRequests.TryRemove( response.Id, out var tcs ) )
		{
			tcs.SetResult( response );
			_logger.LogInformation( "Resource request {Id} completed successfully", response.Id );
		}
	}
}
