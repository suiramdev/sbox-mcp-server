using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;
using ModelContextProtocol.Server;
using SandboxModelContextProtocol.Server.Services.Interfaces;
using SandboxModelContextProtocol.Server.Services.Models;

namespace SandboxModelContextProtocol.Server.Tools;

[McpServerToolType]
public class EditorSceneTool( IEditorToolService editorToolService )
{
	private readonly IEditorToolService _editorToolService = editorToolService;

	[McpServerTool, Description( "Gets the scene in the active editor session." )]
	public async Task<CallEditorToolResponse> GetActiveEditorScene()
	{
		var command = new CallEditorToolRequest()
		{
			Name = nameof( GetActiveEditorScene ),
		};

		return await _editorToolService.CallTool( command );
	}

	[McpServerTool, Description( "Loads a scene in a new editor session from a path." )]
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
}
