using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ModelContextProtocol.Server;

public interface ICommandService
{
    Task<string> ExecuteCommandAsync(string command);
    void RegisterWebSocketConnection(IWebSocketConnection connection);
    void UnregisterWebSocketConnection(IWebSocketConnection connection);
    void HandleResponse(string response);
}

public class CommandService : ICommandService
{
    private readonly ILogger<CommandService> _logger;
    private readonly ConcurrentDictionary<string, IWebSocketConnection> _connections = new();
    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _pendingCommands = new();

    public CommandService(ILogger<CommandService> logger)
    {
        _logger = logger;
    }

    public void RegisterWebSocketConnection(IWebSocketConnection connection)
    {
        var connectionId = Guid.NewGuid().ToString();
        _connections[connectionId] = connection;
        _logger.LogInformation("WebSocket connection registered with ID: {ConnectionId}", connectionId);
    }

    public void UnregisterWebSocketConnection(IWebSocketConnection connection)
    {
        var toRemove = _connections.Where(kvp => kvp.Value == connection).ToList();
        foreach (var kvp in toRemove)
        {
            _connections.TryRemove(kvp.Key, out _);
            _logger.LogInformation("WebSocket connection unregistered: {ConnectionId}", kvp.Key);
        }
    }

    public async Task<string> ExecuteCommandAsync(string command)
    {
        _logger.LogInformation("Executing command: {Command}", command);

        // Find active connections
        var activeConnections = _connections.Values
            .Where(conn => conn.IsConnected)
            .ToList();

        if (activeConnections.Count == 0)
        {
            var errorResult = new { error = "No active s&box connections available" };
            return JsonSerializer.Serialize(errorResult);
        }

        // Generate unique command ID
        var commandId = Guid.NewGuid().ToString();
        
        // Parse the original command and add command ID
        Dictionary<string, object> commandDict;
        try
        {
            commandDict = JsonSerializer.Deserialize<Dictionary<string, object>>(command) ?? new Dictionary<string, object>();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse command JSON: {Command}", command);
            var errorResult = new { error = $"Invalid command JSON: {ex.Message}" };
            return JsonSerializer.Serialize(errorResult);
        }
        
        commandDict["commandId"] = commandId;
        var commandJson = JsonSerializer.Serialize(commandDict);

        // Create task completion source for this command
        var tcs = new TaskCompletionSource<string>();
        _pendingCommands[commandId] = tcs;

        // Send command to all active s&box connections
        var sendTasks = new List<Task>();
        var successfulSends = 0;
        
        foreach (var connection in activeConnections)
        {
            sendTasks.Add(Task.Run(async () =>
            {
                try
                {
                    await connection.SendAsync(commandJson);
                    Interlocked.Increment(ref successfulSends);
                    _logger.LogInformation("Command sent successfully to connection");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send command to connection");
                }
            }));
        }

        try
        {
            // Wait for all send operations to complete
            await Task.WhenAll(sendTasks);
            
            _logger.LogInformation("Command sent to {SuccessfulSends}/{TotalConnections} connections: {CommandJson}", 
                successfulSends, activeConnections.Count, commandJson);

            if (successfulSends == 0)
            {
                _pendingCommands.TryRemove(commandId, out _);
                var errorResult = new { error = "Failed to send command to any connections" };
                return JsonSerializer.Serialize(errorResult);
            }
            
            // Wait for response with timeout
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            cts.Token.Register(() => 
            {
                if (tcs.TrySetCanceled())
                {
                    _pendingCommands.TryRemove(commandId, out _);
                    _logger.LogWarning("Command {CommandId} timed out", commandId);
                }
            });
            
            return await tcs.Task;
        }
        catch (OperationCanceledException)
        {
            _pendingCommands.TryRemove(commandId, out _);
            var errorResult = new { error = "Command timed out after 30 seconds" };
            return JsonSerializer.Serialize(errorResult);
        }
        catch (Exception ex)
        {
            _pendingCommands.TryRemove(commandId, out _);
            _logger.LogError(ex, "Failed to send command to s&box connections");
            var errorResult = new { error = $"Failed to send command: {ex.Message}" };
            return JsonSerializer.Serialize(errorResult);
        }
    }

    public void HandleResponse(string response)
    {
        try
        {
            _logger.LogInformation("Handling response from s&box: {Response}", response);
            
            using var document = JsonDocument.Parse(response);
            var root = document.RootElement;
            
            if (root.TryGetProperty("commandId", out var commandIdElement))
            {
                var commandId = commandIdElement.GetString();
                
                if (!string.IsNullOrEmpty(commandId) && _pendingCommands.TryRemove(commandId, out var tcs))
                {
                    tcs.SetResult(response);
                    _logger.LogInformation("Command {CommandId} completed successfully", commandId);
                    return;
                }
                else
                {
                    _logger.LogWarning("Received response for unknown command ID: {CommandId}", commandId);
                }
            }
            else
            {
                _logger.LogWarning("Received response without command ID: {Response}", response);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse response JSON: {Response}", response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle response: {Response}", response);
        }
    }
} 
