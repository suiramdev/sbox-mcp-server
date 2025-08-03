using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;
using ModelContextProtocol.Server;
using SandboxModelContextProtocol.Server.Services.Interfaces;
using SandboxModelContextProtocol.Server.Services.Models;

namespace SandboxModelContextProtocol.Server.Tools;

[McpServerToolType]
public class GameObjectTool( IToolService toolService )
{
	private readonly IToolService _toolService = toolService;

	[McpServerTool, Description( "Gets a game object by name." )]
	public async Task<CallToolResponse> GetGameObjectByName( string name, string? sceneId = null )
	{
		var command = new CallToolRequest()
		{
			Name = nameof( GetGameObjectByName ),
			Arguments = new Dictionary<string, JsonElement>()
			{
				{ "name", JsonSerializer.SerializeToElement( name ) },
				{ "sceneId", JsonSerializer.SerializeToElement( sceneId ) },
			},
		};

		return await _toolService.CallTool( command );
	}

	[McpServerTool, Description( "Gets a game object by id." )]
	public async Task<CallToolResponse> GetGameObjectById( string id, string? sceneId = null )
	{
		var command = new CallToolRequest()
		{
			Name = nameof( GetGameObjectById ),
			Arguments = new Dictionary<string, JsonElement>()
			{
				{ "id", JsonSerializer.SerializeToElement( id ) },
				{ "sceneId", JsonSerializer.SerializeToElement( sceneId ) },
			},
		};

		return await _toolService.CallTool( command );
	}

	[McpServerTool, Description( "Gets all game objects." )]
	public async Task<CallToolResponse> GetAllGameObjects( string? sceneId = null )
	{
		var command = new CallToolRequest()
		{
			Name = nameof( GetAllGameObjects ),
			Arguments = new Dictionary<string, JsonElement>()
			{
				{ "sceneId", JsonSerializer.SerializeToElement( sceneId ) },
			},
		};

		return await _toolService.CallTool( command );
	}

	[McpServerTool, Description( "Creates a new game object." )]
	public async Task<CallToolResponse> CreateGameObject( string name, string sceneId, string? parentId = null )
	{
		var command = new CallToolRequest()
		{
			Name = nameof( CreateGameObject ),
			Arguments = new Dictionary<string, JsonElement>()
			{
				{ "name", JsonSerializer.SerializeToElement( name ) },
				{ "sceneId", JsonSerializer.SerializeToElement( sceneId ) },
				{ "parentId", JsonSerializer.SerializeToElement( parentId ) },
			},
		};

		return await _toolService.CallTool( command );
	}

	[McpServerTool, Description( "Duplicates a game object." )]
	public async Task<CallToolResponse> DuplicateGameObject( string id, string? sceneId = null, string? parentId = null )
	{
		var command = new CallToolRequest()
		{
			Name = nameof( DuplicateGameObject ),
			Arguments = new Dictionary<string, JsonElement>()
			{
				{ "id", JsonSerializer.SerializeToElement( id ) },
				{ "sceneId", JsonSerializer.SerializeToElement( sceneId ) },
				{ "parentId", JsonSerializer.SerializeToElement( parentId ) },
			},
		};

		return await _toolService.CallTool( command );
	}

	[McpServerTool, Description( "Destroys (deletes) a game object." )]
	public async Task<CallToolResponse> DestroyGameObject( string id, string? sceneId = null )
	{
		var command = new CallToolRequest()
		{
			Name = nameof( DestroyGameObject ),
			Arguments = new Dictionary<string, JsonElement>()
			{
				{ "id", JsonSerializer.SerializeToElement( id ) },
				{ "sceneId", JsonSerializer.SerializeToElement( sceneId ) },
			},
		};

		return await _toolService.CallTool( command );
	}

	// Transform Commands
	[McpServerTool, Description( "Sets the world position of a game object." )]
	public async Task<CallToolResponse> SetGameObjectWorldPosition( string id, float x, float y, float z, string? sceneId = null )
	{
		var command = new CallToolRequest()
		{
			Name = nameof( SetGameObjectWorldPosition ),
			Arguments = new Dictionary<string, JsonElement>()
			{
				{ "id", JsonSerializer.SerializeToElement( id ) },
				{ "x", JsonSerializer.SerializeToElement( x ) },
				{ "y", JsonSerializer.SerializeToElement( y ) },
				{ "z", JsonSerializer.SerializeToElement( z ) },
				{ "sceneId", JsonSerializer.SerializeToElement( sceneId ) },
			},
		};

		return await _toolService.CallTool( command );
	}

	[McpServerTool, Description( "Sets the world rotation of a game object." )]
	public async Task<CallToolResponse> SetGameObjectWorldRotation( string id, float x, float y, float z, float w, string? sceneId = null )
	{
		var command = new CallToolRequest()
		{
			Name = nameof( SetGameObjectWorldRotation ),
			Arguments = new Dictionary<string, JsonElement>()
			{
				{ "id", JsonSerializer.SerializeToElement( id ) },
				{ "x", JsonSerializer.SerializeToElement( x ) },
				{ "y", JsonSerializer.SerializeToElement( y ) },
				{ "z", JsonSerializer.SerializeToElement( z ) },
				{ "w", JsonSerializer.SerializeToElement( w ) },
				{ "sceneId", JsonSerializer.SerializeToElement( sceneId ) },
			},
		};

		return await _toolService.CallTool( command );
	}

	[McpServerTool, Description( "Sets the world scale of a game object." )]
	public async Task<CallToolResponse> SetGameObjectWorldScale( string id, float x, float y, float z, string? sceneId = null )
	{
		var command = new CallToolRequest()
		{
			Name = nameof( SetGameObjectWorldScale ),
			Arguments = new Dictionary<string, JsonElement>()
			{
				{ "id", JsonSerializer.SerializeToElement( id ) },
				{ "x", JsonSerializer.SerializeToElement( x ) },
				{ "y", JsonSerializer.SerializeToElement( y ) },
				{ "z", JsonSerializer.SerializeToElement( z ) },
				{ "sceneId", JsonSerializer.SerializeToElement( sceneId ) },
			},
		};

		return await _toolService.CallTool( command );
	}

	[McpServerTool, Description( "Sets the local position of a game object." )]
	public async Task<CallToolResponse> SetGameObjectLocalPosition( string id, float x, float y, float z, string? sceneId = null )
	{
		var command = new CallToolRequest()
		{
			Name = nameof( SetGameObjectLocalPosition ),
			Arguments = new Dictionary<string, JsonElement>()
			{
				{ "id", JsonSerializer.SerializeToElement( id ) },
				{ "x", JsonSerializer.SerializeToElement( x ) },
				{ "y", JsonSerializer.SerializeToElement( y ) },
				{ "z", JsonSerializer.SerializeToElement( z ) },
				{ "sceneId", JsonSerializer.SerializeToElement( sceneId ) },
			},
		};

		return await _toolService.CallTool( command );
	}

	[McpServerTool, Description( "Sets the local rotation of a game object." )]
	public async Task<CallToolResponse> SetGameObjectLocalRotation( string id, float x, float y, float z, float w, string? sceneId = null )
	{
		var command = new CallToolRequest()
		{
			Name = nameof( SetGameObjectLocalRotation ),
			Arguments = new Dictionary<string, JsonElement>()
			{
				{ "id", JsonSerializer.SerializeToElement( id ) },
				{ "x", JsonSerializer.SerializeToElement( x ) },
				{ "y", JsonSerializer.SerializeToElement( y ) },
				{ "z", JsonSerializer.SerializeToElement( z ) },
				{ "w", JsonSerializer.SerializeToElement( w ) },
				{ "sceneId", JsonSerializer.SerializeToElement( sceneId ) },
			},
		};

		return await _toolService.CallTool( command );
	}

	[McpServerTool, Description( "Sets the local scale of a game object." )]
	public async Task<CallToolResponse> SetGameObjectLocalScale( string id, float x, float y, float z, string? sceneId = null )
	{
		var command = new CallToolRequest()
		{
			Name = nameof( SetGameObjectLocalScale ),
			Arguments = new Dictionary<string, JsonElement>()
			{
				{ "id", JsonSerializer.SerializeToElement( id ) },
				{ "x", JsonSerializer.SerializeToElement( x ) },
				{ "y", JsonSerializer.SerializeToElement( y ) },
				{ "z", JsonSerializer.SerializeToElement( z ) },
				{ "sceneId", JsonSerializer.SerializeToElement( sceneId ) },
			},
		};

		return await _toolService.CallTool( command );
	}

	// Hierarchy Commands
	[McpServerTool, Description( "Sets the parent of a game object." )]
	public async Task<CallToolResponse> SetGameObjectParent( string id, string? parentId, string? sceneId = null )
	{
		var command = new CallToolRequest()
		{
			Name = nameof( SetGameObjectParent ),
			Arguments = new Dictionary<string, JsonElement>()
			{
				{ "id", JsonSerializer.SerializeToElement( id ) },
				{ "parentId", JsonSerializer.SerializeToElement( parentId ) },
				{ "sceneId", JsonSerializer.SerializeToElement( sceneId ) },
			},
		};

		return await _toolService.CallTool( command );
	}

	[McpServerTool, Description( "Gets all child game objects of a game object." )]
	public async Task<CallToolResponse> GetGameObjectChildren( string id, string? sceneId = null )
	{
		var command = new CallToolRequest()
		{
			Name = nameof( GetGameObjectChildren ),
			Arguments = new Dictionary<string, JsonElement>()
			{
				{ "id", JsonSerializer.SerializeToElement( id ) },
				{ "sceneId", JsonSerializer.SerializeToElement( sceneId ) },
			},
		};

		return await _toolService.CallTool( command );
	}

	[McpServerTool, Description( "Gets the parent of a game object." )]
	public async Task<CallToolResponse> GetGameObjectParent( string id, string? sceneId = null )
	{
		var command = new CallToolRequest()
		{
			Name = nameof( GetGameObjectParent ),
			Arguments = new Dictionary<string, JsonElement>()
			{
				{ "id", JsonSerializer.SerializeToElement( id ) },
				{ "sceneId", JsonSerializer.SerializeToElement( sceneId ) },
			},
		};

		return await _toolService.CallTool( command );
	}

	// Property Commands
	[McpServerTool, Description( "Sets the name of a game object." )]
	public async Task<CallToolResponse> SetGameObjectName( string id, string name, string? sceneId = null )
	{
		var command = new CallToolRequest()
		{
			Name = nameof( SetGameObjectName ),
			Arguments = new Dictionary<string, JsonElement>()
			{
				{ "id", JsonSerializer.SerializeToElement( id ) },
				{ "name", JsonSerializer.SerializeToElement( name ) },
				{ "sceneId", JsonSerializer.SerializeToElement( sceneId ) },
			},
		};

		return await _toolService.CallTool( command );
	}

	[McpServerTool, Description( "Sets the enabled state of a game object." )]
	public async Task<CallToolResponse> SetGameObjectEnabled( string id, bool enabled, string? sceneId = null )
	{
		var command = new CallToolRequest()
		{
			Name = nameof( SetGameObjectEnabled ),
			Arguments = new Dictionary<string, JsonElement>()
			{
				{ "id", JsonSerializer.SerializeToElement( id ) },
				{ "enabled", JsonSerializer.SerializeToElement( enabled ) },
				{ "sceneId", JsonSerializer.SerializeToElement( sceneId ) },
			},
		};

		return await _toolService.CallTool( command );
	}

	// Component Commands
	[McpServerTool, Description( "Adds a component to a game object." )]
	public async Task<CallToolResponse> AddGameObjectComponent( string id, string componentType, string? sceneId = null )
	{
		var command = new CallToolRequest()
		{
			Name = nameof( AddGameObjectComponent ),
			Arguments = new Dictionary<string, JsonElement>()
			{
				{ "id", JsonSerializer.SerializeToElement( id ) },
				{ "componentType", JsonSerializer.SerializeToElement( componentType ) },
				{ "sceneId", JsonSerializer.SerializeToElement( sceneId ) },
			},
		};

		return await _toolService.CallTool( command );
	}

	[McpServerTool, Description( "Removes a component from a game object." )]
	public async Task<CallToolResponse> RemoveGameObjectComponent( string id, string componentType, string? sceneId = null )
	{
		var command = new CallToolRequest()
		{
			Name = nameof( RemoveGameObjectComponent ),
			Arguments = new Dictionary<string, JsonElement>()
			{
				{ "id", JsonSerializer.SerializeToElement( id ) },
				{ "componentType", JsonSerializer.SerializeToElement( componentType ) },
				{ "sceneId", JsonSerializer.SerializeToElement( sceneId ) },
			},
		};

		return await _toolService.CallTool( command );
	}

	[McpServerTool, Description( "Gets all components of a game object." )]
	public async Task<CallToolResponse> GetGameObjectComponents( string id, string? sceneId = null )
	{
		var command = new CallToolRequest()
		{
			Name = nameof( GetGameObjectComponents ),
			Arguments = new Dictionary<string, JsonElement>()
			{
				{ "id", JsonSerializer.SerializeToElement( id ) },
				{ "sceneId", JsonSerializer.SerializeToElement( sceneId ) },
			},
		};

		return await _toolService.CallTool( command );
	}

	[McpServerTool, Description( "Gets a specific component of a game object." )]
	public async Task<CallToolResponse> GetGameObjectComponent( string id, string componentType, string? sceneId = null )
	{
		var command = new CallToolRequest()
		{
			Name = nameof( GetGameObjectComponent ),
			Arguments = new Dictionary<string, JsonElement>()
			{
				{ "id", JsonSerializer.SerializeToElement( id ) },
				{ "componentType", JsonSerializer.SerializeToElement( componentType ) },
				{ "sceneId", JsonSerializer.SerializeToElement( sceneId ) },
			},
		};

		return await _toolService.CallTool( command );
	}

	[McpServerTool, Description( "Sets properties of a component on a game object." )]
	public async Task<CallToolResponse> SetGameObjectComponentProperty( string id, string componentType, string propertyName, JsonElement value, string? sceneId = null )
	{
		var command = new CallToolRequest()
		{
			Name = nameof( SetGameObjectComponentProperty ),
			Arguments = new Dictionary<string, JsonElement>()
			{
				{ "id", JsonSerializer.SerializeToElement( id ) },
				{ "componentType", JsonSerializer.SerializeToElement( componentType ) },
				{ "propertyName", JsonSerializer.SerializeToElement( propertyName ) },
				{ "value", value },
				{ "sceneId", JsonSerializer.SerializeToElement( sceneId ) },
			},
		};

		return await _toolService.CallTool( command );
	}
}
