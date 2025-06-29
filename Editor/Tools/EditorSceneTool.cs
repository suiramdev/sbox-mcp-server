using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Editor;
using Sandbox;
using SandboxModelContextProtocol.Editor.Commands.Attributes;

namespace SandboxModelContextProtocol.Editor.Tools;

[McpEditorToolType]
public class EditorSceneTool
{
	[McpEditorTool]
	public static JsonObject GetActiveEditorScene()
	{
		Scene? scene = SceneEditorSession.Active.Scene ?? throw new InvalidOperationException( "No scene found" );
		return scene.Serialize();
	}

	[McpEditorTool]
	public static async Task LoadEditorSceneFromPath( string scenePath )
	{
		if ( !ResourceLibrary.TryGet( scenePath, out SceneFile sceneFile ) )
		{
			throw new InvalidOperationException( $"Scene file {scenePath} not found" );
		}

		await EditorScene.LoadFromScene( sceneFile );
	}

	[McpEditorTool]
	public static void SaveAllEditorSessions()
	{
		EditorScene.SaveAllSessions();
	}

	[McpEditorTool]
	public static void SaveEditorSession()
	{
		EditorScene.SaveSession();
	}
}