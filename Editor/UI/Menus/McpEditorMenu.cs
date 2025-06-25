using Editor;
using SandboxModelContextProtocol.Editor.Connection;
using SandboxModelContextProtocol.Editor.UI.Widgets;

namespace SandboxModelContextProtocol.Editor.UI.Menus;

public static class McpEditorMenu
{
	[Menu( "Editor", "MCP/Connect to MCP Server", "link" )]
	public static void ConnectToMcp()
	{
		McpConnectionOverlay.Reset();

		_ = McpConnectionManager.Connect();
	}
}
