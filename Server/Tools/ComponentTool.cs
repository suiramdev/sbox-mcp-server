using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;
using ModelContextProtocol.Server.Services;

namespace ModelContextProtocol.Server.Tools;

/// <summary>
/// Tool for managing s&box scene components via WebSocket communication
/// </summary>
[McpServerToolType]
public class ComponentTool( ICommandService commandService )
{
	private readonly ICommandService _commandService = commandService;

	[McpServerTool, Description( "Creates a new component on a game object. Requires componentType and gameObjectId." )]
	public async Task<string> CreateComponent( string componentType, string gameObjectId )
	{
		var command = JsonSerializer.Serialize( new
		{
			action = "create_component",
			componentType,
			gameObjectId
		} );

		return await _commandService.ExecuteCommandAsync( command );
	}

	[McpServerTool, Description( "Gets all components attached to a game object. Requires gameObjectId." )]
	public async Task<string> GetComponents( string gameObjectId )
	{
		var command = JsonSerializer.Serialize( new
		{
			action = "get_components",
			gameObjectId
		} );

		return await _commandService.ExecuteCommandAsync( command );
	}

	[McpServerTool, Description( "Gets a specific component by type from a game object. Requires componentType and gameObjectId." )]
	public async Task<string> GetComponent( string componentType, string gameObjectId )
	{
		var command = JsonSerializer.Serialize( new
		{
			action = "get_component",
			componentType,
			gameObjectId
		} );

		return await _commandService.ExecuteCommandAsync( command );
	}

	[McpServerTool, Description( "Removes a component from a game object. Requires componentType and gameObjectId." )]
	public async Task<string> RemoveComponent( string componentType, string gameObjectId )
	{
		var command = JsonSerializer.Serialize( new
		{
			action = "remove_component",
			componentType,
			gameObjectId
		} );

		return await _commandService.ExecuteCommandAsync( command );
	}

	[McpServerTool, Description( "Sets a property value on a component. Requires componentType, propertyName, propertyValue, and gameObjectId." )]
	public async Task<string> SetComponentProperty( string componentType, string propertyName, string propertyValue, string gameObjectId )
	{
		var command = JsonSerializer.Serialize( new
		{
			action = "set_component_property",
			componentType,
			propertyName,
			propertyValue,
			gameObjectId
		} );

		return await _commandService.ExecuteCommandAsync( command );
	}

	// ... (add async/await to all other methods similarly)
}
