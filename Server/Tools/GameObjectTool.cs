using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ModelContextProtocol.Server;
using ModelContextProtocol.Protocol;
using SandboxModelContextProtocol.Server.Services.Interfaces;
using SandboxModelContextProtocol.Server.Services.Models;

namespace SandboxModelContextProtocol.Server.Tools;

/// <summary>
/// Tool for managing s&box scene components via WebSocket communication
/// </summary>
[McpServerToolType]
public class GameObjectTool( ICommandService commandService )
{
	private readonly ICommandService _commandService = commandService;

	[McpServerTool, Description( "Finds game objects by name. Requires name." )]
	public async Task<CallToolResponse> FindGameObjectsByName( string name )
	{
		var command = new CommandRequest()
		{
			Command = "find_game_objects_by_name",
			Arguments = new Dictionary<string, object>()
			{
				{ "name", name }
			}
		};

		var response = await _commandService.ExecuteCommandAsync( command );

		return response.ToCallToolResponse();
	}
}
