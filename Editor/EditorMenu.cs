using Editor;

namespace ModelContextProtocol;

public static class EditorMenu 
{
	[Menu( "Editor", "MCP/Connect to MCP Server", "link" )]
	public static void OnRestart()
	{
		WebSocketClient.Connect().Wait();
	}
}
