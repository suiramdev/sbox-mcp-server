using Editor;
using System.Threading.Tasks;
using SandboxModelContextProtocol.Editor.Services;

namespace SandboxModelContextProtocol.Editor;

public static class EditorMenu
{
	[Menu( "Editor", "MCP/Connect to MCP Server", "link" )]
	public static void OnRestart()
	{
		Task.Run( () => WebSocketService.Connect() ).ContinueWith( ( task ) =>
		{
			if ( task.IsFaulted )
			{
				Log.Error( $"Error connecting to MCP Server: {task.Exception?.Message}" );
				EditorUtility.PlayRawSound( "sounds/editor/fail.wav" );
			}
			else
			{
				Log.Info( "Connected to MCP Server" );
				EditorUtility.PlayRawSound( "sounds/editor/success.wav" );
			}
		} );
	}
}
