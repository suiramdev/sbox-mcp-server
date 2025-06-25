using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;
using ModelContextProtocol.Server;
using SandboxModelContextProtocol.Server.Services.Interfaces;
using SandboxModelContextProtocol.Server.Services.Models;

namespace SandboxModelContextProtocol.Server.Tools;

/// <summary>
/// Tool for managing s&box scene components via WebSocket communication
/// </summary>
[McpServerToolType]
public class SceneTool( IEditorToolService editorToolService )
{
	private readonly IEditorToolService _editorToolService = editorToolService;

	[McpServerTool, Description( "Gets the active scene." )]
	public async Task<CallEditorToolResponse> GetActiveScene()
	{
		var command = new CallEditorToolRequest()
		{
			Name = nameof( GetActiveScene ),
		};

		return await _editorToolService.CallTool( command );
	}
}
