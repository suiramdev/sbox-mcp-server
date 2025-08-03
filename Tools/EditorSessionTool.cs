using System.Threading.Tasks;
using System.ComponentModel;
using ModelContextProtocol.Server;
using SandboxModelContextProtocol.Server.Services.Interfaces;
using SandboxModelContextProtocol.Server.Services.Models;

namespace SandboxModelContextProtocol.Server.Tools;

[McpServerToolType]
public class EditorSessionTool( IToolService toolService )
{
	private readonly IToolService _toolService = toolService;

	[McpServerTool, Description( "Gets the active editor session." )]
	public async Task<CallToolResponse> GetActiveEditorSession()
	{
		var command = new CallToolRequest()
		{
			Name = nameof( GetActiveEditorSession ),
		};

		return await _toolService.CallTool( command );
	}

	[McpServerTool, Description( "Gets all editor sessions." )]
	public async Task<CallToolResponse> GetAllEditorSessions()
	{
		var command = new CallToolRequest()
		{
			Name = nameof( GetAllEditorSessions ),
		};

		return await _toolService.CallTool( command );
	}

	[McpServerTool, Description( "Saves all editor sessions." )]
	public async Task<CallToolResponse> SaveAllEditorSessions()
	{
		var command = new CallToolRequest()
		{
			Name = nameof( SaveAllEditorSessions ),
		};

		return await _toolService.CallTool( command );
	}

	[McpServerTool, Description( "Saves the active editor session." )]
	public async Task<CallToolResponse> SaveActiveEditorSession()
	{
		var command = new CallToolRequest()
		{
			Name = nameof( SaveActiveEditorSession ),
		};

		return await _toolService.CallTool( command );
	}
}
