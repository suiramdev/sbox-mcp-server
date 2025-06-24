using System.Collections.Generic;
using Editor;
using Sandbox;
using SandboxModelContextProtocol.Editor.Connection;

namespace SandboxModelContextProtocol.Editor;

public static class MCPConnectionEditorMenu
{
	[Menu( "Editor", "MCP/Connect to MCP Server", "link" )]
	public static void ConnectToMCP()
	{
		MCPConnectionOverlay.AddToOverlay();

		_ = MCPConnectionManager.Connect();
	}
}
