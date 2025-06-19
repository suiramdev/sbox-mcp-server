using System.Threading.Tasks;
using System.ComponentModel;
using ModelContextProtocol.Server;
using SandboxModelContextProtocol.Server.Services.Interfaces;
using SandboxModelContextProtocol.Server.Services.Models;

namespace SandboxModelContextProtocol.Server.Tools;

[McpServerToolType]
public class EditorSessionTool( IEditorToolService editorToolService )
{
	private readonly IEditorToolService _editorToolService = editorToolService;

	[McpServerTool, Description( "Gets the active editor session." )]
	public async Task<CallEditorToolResponse> GetActiveEditorSession()
	{
		var command = new CallEditorToolRequest()
		{
			Name = nameof( GetActiveEditorSession ),
		};

		return await _editorToolService.CallTool( command );
	}

	[McpServerTool, Description( "Gets all editor sessions." )]
	public async Task<CallEditorToolResponse> GetAllEditorSessions()
	{
		var command = new CallEditorToolRequest()
		{
			Name = nameof( GetAllEditorSessions ),
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
	public async Task<CallEditorToolResponse> SaveActiveEditorSession()
	{
		var command = new CallEditorToolRequest()
		{
			Name = nameof( SaveActiveEditorSession ),
		};

		return await _editorToolService.CallTool( command );
	}
}