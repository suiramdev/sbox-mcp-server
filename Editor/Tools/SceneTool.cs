using System;
using System.Text.Json.Nodes;
using Editor;
using Sandbox;
using SandboxModelContextProtocol.Editor.Commands.Attributes;

namespace SandboxModelContextProtocol.Editor.Tools;

[McpEditorToolType]
public class SceneTool
{
	[McpEditorTool]
	public static JsonObject GetActiveScene()
	{
		Scene? scene = SceneEditorSession.Active.Scene;
		if ( scene == null )
		{
			throw new InvalidOperationException( "No scene found" );
		}

		return scene.Serialize();
	}
}