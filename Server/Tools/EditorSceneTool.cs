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
[McpServerToolType, Description( "Editor scene tools" )]
public class EditorSceneTool( IEditorToolService editorToolService )
{
	private readonly IEditorToolService _editorToolService = editorToolService;

	[McpServerTool, Description( "Gets the active editor scene." )]
	public async Task<CallEditorToolResponse> GetActiveEditorScene()
	{
		var command = new CallEditorToolRequest()
		{
			Name = nameof( GetActiveEditorScene ),
		};

		return await _editorToolService.CallTool( command );
	}

	[McpServerTool, Description( "Loads a scene from a path." )]
	public async Task<CallEditorToolResponse> LoadEditorSceneFromPath( string path )
	{
		var command = new CallEditorToolRequest()
		{
			Name = nameof( LoadEditorSceneFromPath ),
			Arguments = new Dictionary<string, JsonElement>()
			{
				{ "path", JsonSerializer.SerializeToElement( path ) },
			},
		};

		return await _editorToolService.CallTool( command );
	}

	[McpServerTool, Description( "Saves all editor sessions." )]
	public async Task<CallEditorToolResponse> SaveAllEditorSessions()
	{
		var command = new CallEditorToolRequest()
		{
			Name = nameof( SaveAllEditorSessions ),
		};

		return await _editorToolService.CallTool( command );
	}

	[McpServerTool, Description( "Saves the active editor session." )]
	public async Task<CallEditorToolResponse> SaveEditorSession()
	{
		var command = new CallEditorToolRequest()
		{
			Name = nameof( SaveEditorSession ),
		};

		return await _editorToolService.CallTool( command );
	}
}
