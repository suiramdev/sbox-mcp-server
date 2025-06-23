using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ModelContextProtocol.Server;
using ModelContextProtocol.Protocol;
using SandboxModelContextProtocol.Server.Models;
using SandboxModelContextProtocol.Server.Services.Interfaces;

namespace SandboxModelContextProtocol.Server.Tools;

/// <summary>
/// Tool for managing s&box scene components via WebSocket communication
/// </summary>
[McpServerToolType]
public class ComponentTool( ICommandService commandService )
{
	private readonly ICommandService _commandService = commandService;

	[McpServerTool, Description( "Creates a new component on a game object. Requires componentType and gameObjectId." )]
	public async Task<CallToolResponse> CreateComponent( string componentType, string gameObjectId )
	{
		var command = new CommandRequest()
		{
			Command = "create_component",
			Arguments = new Dictionary<string, object>()
			{
				{ "componentType", componentType },
				{ "gameObjectId", gameObjectId }
			}
		};

		var response = await _commandService.ExecuteCommandAsync( command );

		return response.ToCallToolResponse();
	}

	[McpServerTool, Description( "Gets all components attached to a game object. Requires gameObjectId." )]
	public async Task<CallToolResponse> GetComponents( string gameObjectId )
	{
		var command = new CommandRequest()
		{
			Command = "get_components",
			Arguments = new Dictionary<string, object>()
			{
				{ "gameObjectId", gameObjectId }
			}
		};

		var response = await _commandService.ExecuteCommandAsync( command );

		return response.ToCallToolResponse();
	}

	[McpServerTool, Description( "Gets a specific component by type from a game object. Requires componentType and gameObjectId." )]
	public async Task<CallToolResponse> GetComponent( string componentType, string gameObjectId )
	{
		var command = new CommandRequest()
		{
			Command = "get_component",
			Arguments = new Dictionary<string, object>()
			{
				{ "componentType", componentType },
				{ "gameObjectId", gameObjectId }
			}
		};

		var response = await _commandService.ExecuteCommandAsync( command );

		return response.ToCallToolResponse();
	}

	[McpServerTool, Description( "Removes a component from a game object. Requires componentType and gameObjectId." )]
	public async Task<CallToolResponse> RemoveComponent( string componentType, string gameObjectId )
	{
		var command = new CommandRequest()
		{
			Command = "remove_component",
			Arguments = new Dictionary<string, object>()
			{
				{ "componentType", componentType },
				{ "gameObjectId", gameObjectId }
			}
		};

		var response = await _commandService.ExecuteCommandAsync( command );

		return response.ToCallToolResponse();
	}

	[McpServerTool, Description( "Sets a property value on a component. Requires componentType, propertyName, propertyValue, and gameObjectId." )]
	public async Task<CallToolResponse> SetComponentProperty( string componentType, string propertyName, string propertyValue, string gameObjectId )
	{
		var command = new CommandRequest()
		{
			Command = "set_component_property",
			Arguments = new Dictionary<string, object>()
			{
				{ "componentType", componentType },
				{ "propertyName", propertyName },
				{ "propertyValue", propertyValue },
				{ "gameObjectId", gameObjectId }
			}
		};

		var response = await _commandService.ExecuteCommandAsync( command );

		return response.ToCallToolResponse();
	}
}
