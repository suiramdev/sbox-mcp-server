using Editor;
using System.Threading.Tasks;
using SandboxModelContextProtocol.Editor.Connection;

namespace SandboxModelContextProtocol.Editor;

public static class ConnectionEditorMenu
{
	[Menu( "Editor", "MCP/Connect to MCP Server", "link" )]
	public static void ConnectToMCP()
	{
		_ = MCPConnectionManager.Connect();
	}
}
