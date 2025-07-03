using System;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Editor;
using Sandbox;
using SandboxModelContextProtocol.Editor.Commands.Attributes;

namespace SandboxModelContextProtocol.Editor.Tools;

[McpEditorToolType]
public class GameObjectTool
{
	[McpEditorTool]
	public static JsonObject GetGameObjectByName( string name, string? sceneId = null )
	{
		var scene = GetScene( sceneId );

		var gameObject = scene.GetAllObjects( false ).FirstOrDefault( go => go.Name == name );
		return gameObject?.Serialize() ?? throw new InvalidOperationException( $"GameObject with name {name} not found" );
	}

	[McpEditorTool]
	public static JsonObject GetGameObjectById( string id, string? sceneId = null )
	{
		var scene = GetScene( sceneId );

		var gameObject = scene.GetAllObjects( false ).FirstOrDefault( go => go.Id == new Guid( id ) );
		return gameObject?.Serialize() ?? throw new InvalidOperationException( $"GameObject with id {id} not found" );
	}

	[McpEditorTool]
	public static JsonObject GetAllGameObjects( string? sceneId = null )
	{
		var scene = GetScene( sceneId );
		var gameObjects = scene.GetAllObjects( false );
		return new JsonObject( gameObjects.SelectMany( go => go.Serialize() ) );
	}

	[McpEditorTool]
	public static JsonObject CreateGameObject( string name, string sceneId, string? parentId = null )
	{
		var scene = GetScene( sceneId );

		GameObject? parent = null;
		if ( parentId != null )
		{
			parent = GetGameObjectById( new Guid( parentId ), scene );
		}

		var gameObject = scene.CreateObject();
		gameObject.Name = name;
		gameObject.SetParent( parent );

		return gameObject.Serialize();
	}

	[McpEditorTool]
	public static JsonObject DuplicateGameObject( string id, string? sceneId = null, string? parentId = null )
	{
		var scene = GetScene( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		GameObject? parent = null;
		if ( parentId != null )
		{
			// Check if parent exists
			parent = GetGameObjectById( new Guid( parentId ), scene );
		}

		var duplicate = gameObject.Clone();
		duplicate.Name = gameObject.Name + " (Copy)";
		duplicate.SetParent( parent );

		return duplicate.Serialize();
	}

	[McpEditorTool]
	public static bool DestroyGameObject( string id, string? sceneId = null )
	{
		var scene = GetScene( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		gameObject.Destroy();

		return true;
	}

	// Transform Commands
	[McpEditorTool]
	public static JsonObject SetGameObjectWorldPosition( string id, float x, float y, float z, string? sceneId = null )
	{
		var scene = GetScene( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		gameObject.WorldPosition = new Vector3( x, y, z );

		return gameObject.Serialize();
	}

	[McpEditorTool]
	public static JsonObject SetGameObjectWorldRotation( string id, float x, float y, float z, float w, string? sceneId = null )
	{
		var scene = GetScene( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		gameObject.WorldRotation = new Rotation( x, y, z, w );

		return gameObject.Serialize();
	}

	[McpEditorTool]
	public static JsonObject SetGameObjectWorldScale( string id, float x, float y, float z, string? sceneId = null )
	{
		var scene = GetScene( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		gameObject.WorldScale = new Vector3( x, y, z );

		return gameObject.Serialize();
	}

	[McpEditorTool]
	public static JsonObject SetGameObjectLocalPosition( string id, float x, float y, float z, string? sceneId = null )
	{
		var scene = GetScene( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		gameObject.LocalPosition = new Vector3( x, y, z );

		return gameObject.Serialize();
	}

	[McpEditorTool]
	public static JsonObject SetGameObjectLocalRotation( string id, float x, float y, float z, float w, string? sceneId = null )
	{
		var scene = GetScene( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		gameObject.LocalRotation = new Rotation( x, y, z, w );

		return gameObject.Serialize();
	}

	[McpEditorTool]
	public static JsonObject SetGameObjectLocalScale( string id, float x, float y, float z, string? sceneId = null )
	{
		var scene = GetScene( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		gameObject.LocalScale = new Vector3( x, y, z );

		return gameObject.Serialize();
	}

	// Hierarchy Commands
	[McpEditorTool]
	public static JsonObject SetGameObjectParent( string id, string? parentId, string? sceneId = null )
	{
		var scene = GetScene( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		GameObject? parent = null;
		if ( parentId != null )
		{
			parent = GetGameObjectById( new Guid( parentId ), scene );
		}

		gameObject.SetParent( parent );

		return gameObject.Serialize();
	}

	[McpEditorTool]
	public static JsonObject GetGameObjectChildren( string id, string? sceneId = null )
	{
		var scene = GetScene( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		return new JsonObject( gameObject.Children.SelectMany( go => go.Serialize() ) );
	}

	[McpEditorTool]
	public static JsonObject GetGameObjectParent( string id, string? sceneId = null )
	{
		var scene = GetScene( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		return gameObject.Parent?.Serialize() ?? throw new InvalidOperationException( $"GameObject with id {id} has no parent" );
	}

	// Property Commands
	[McpEditorTool]
	public static JsonObject SetGameObjectName( string id, string name, string? sceneId = null )
	{
		var scene = GetScene( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		gameObject.Name = name;

		return gameObject.Serialize();
	}

	[McpEditorTool]
	public static JsonObject SetGameObjectEnabled( string id, bool enabled, string? sceneId = null )
	{
		var scene = GetScene( sceneId );

		var gameObject = GetGameObjectById( new Guid( id ), scene );

		gameObject.Enabled = enabled;

		return gameObject.Serialize();
	}

	// Component Commands
	[McpEditorTool]
	public static JsonObject AddGameObjectComponent( string id, string componentType, string? sceneId = null )
	{
		throw new NotImplementedException();
	}

	[McpEditorTool]
	public static JsonObject RemoveGameObjectComponent( string id, string componentType, string? sceneId = null )
	{
		throw new NotImplementedException();
	}

	[McpEditorTool]
	public static JsonObject GetGameObjectComponents( string id, string? sceneId = null )
	{
		throw new NotImplementedException();
	}

	[McpEditorTool]
	public static JsonObject GetGameObjectComponent( string id, string componentType, string? sceneId = null )
	{
		throw new NotImplementedException();
	}

	[McpEditorTool]
	public static JsonObject SetGameObjectComponentProperty( string id, string componentType, string propertyName, JsonNode value, string? sceneId = null )
	{
		throw new NotImplementedException();
	}

	private static Scene GetScene( string? sceneId )
	{
		var scene = SceneEditorSession.Active.Scene;
		if ( sceneId != null )
		{
			scene = SceneEditorSession.All.FirstOrDefault( s => s.Scene.Id == new Guid( sceneId ) )?.Scene;
		}

		return scene ?? throw new InvalidOperationException( sceneId == null ? "No active scene found" : $"Scene with id {sceneId} not found" );
	}

	private static GameObject GetGameObjectById( Guid guid, Scene scene )
	{
		return scene.GetAllObjects( false ).FirstOrDefault( go => go.Id == guid )
			?? throw new InvalidOperationException( $"GameObject with id {guid} not found" );
	}

	private static GameObject GetGameObjectByName( string name, Scene scene )
	{
		return scene.GetAllObjects( false ).FirstOrDefault( go => go.Name == name )
			?? throw new InvalidOperationException( $"GameObject with name {name} not found" );
	}
}
