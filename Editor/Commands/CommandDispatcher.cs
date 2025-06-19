using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Sandbox;

namespace ModelContextProtocol.Commands;

/// <summary>
/// Dispatches commands to appropriate handlers based on action type
/// </summary>
public class CommandDispatcher
{
	private readonly List<ICommandHandler> _handlers;

	public CommandDispatcher()
	{
		_handlers = new List<ICommandHandler>
		{
			new ComponentCommandHandler()
			// Add more handlers here as needed
		};
	}

	/// <summary>
	/// Processes a command and returns the result
	/// </summary>
	/// <param name="command">The command JSON string</param>
	/// <returns>The result of command execution</returns>
	public async Task<string> ProcessCommandAsync(string command)
	{
		try
		{
			Log.Info($"Processing command: {command}");

			// Parse command to get action type
			var commandData = JsonSerializer.Deserialize<Dictionary<string, object>>(command);
			
			if (!commandData.TryGetValue("action", out var actionObj) || actionObj is not JsonElement actionElement)
			{
				return CreateErrorResponse("Missing or invalid action in command");
			}

			var action = actionElement.GetString();
			
			// Find appropriate handler
			var handler = _handlers.FirstOrDefault(h => h.CanHandle(action));
			
			if (handler == null)
			{
				return CreateErrorResponse($"No handler found for action: {action}");
			}

			// Execute command
			var result = await handler.HandleAsync(command);
			Log.Info($"Command processed successfully: {action}");
			
			return result;
		}
		catch (Exception ex)
		{
			Log.Error($"Error processing command: {ex.Message}");
			return CreateErrorResponse($"Command processing failed: {ex.Message}");
		}
	}

	/// <summary>
	/// Gets all supported actions from registered handlers
	/// </summary>
	/// <returns>List of supported action types</returns>
	public List<string> GetSupportedActions()
	{
		var actions = new List<string>();
		
		// This is a simplified approach - in practice you might want handlers to expose their supported actions
		var componentHandler = _handlers.OfType<ComponentCommandHandler>().FirstOrDefault();
		if (componentHandler != null)
		{
			actions.AddRange(new[]
			{
				"create_component",
				"get_components",
				"get_component", 
				"remove_component",
				"set_component_property"
			});
		}

		return actions;
	}

	private string CreateErrorResponse(string error, string commandId = null)
	{
		return JsonSerializer.Serialize(new
		{
			commandId = commandId ?? Guid.NewGuid().ToString(),
			success = false,
			error = error
		});
	}
} 
