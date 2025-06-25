using Sandbox;
using System.Collections.Generic;
using System.Threading.Tasks;
using SandboxModelContextProtocol.Editor.Commands.Interfaces;
using SandboxModelContextProtocol.Editor.Commands.Models;
using System;
using System.Linq;
using System.Text.Json;
using Editor;

namespace SandboxModelContextProtocol.Editor.Commands;

public class ComponentCommandHandler : ICommandHandler
{
	private readonly HashSet<string> _supportedCommands =
	[
		"create_component",
		"get_components",
		"get_component",
		"remove_component",
	];

	public bool CanHandle( CommandRequest request )
	{
		return _supportedCommands.Contains( request.Command );
	}

	public Task<CommandResponse> HandleAsync( CommandRequest request )
	{
		try
		{
			var response = request.Command switch
			{
				"create_component" => Task.FromResult( HandleCreateComponent( request ) ),
				"get_components" => Task.FromResult( HandleGetComponents( request ) ),
				_ => Task.FromResult( new CommandResponse()
				{
					CommandId = request.CommandId,
					Content = $"Unsupported action: {request.Command}",
					IsError = true
				} )
			};

			return response;
		}
		catch ( Exception ex )
		{
			Log.Error( $"Error handling command '{request.Command}': {ex.Message}" );
			return Task.FromResult( new CommandResponse()
			{
				CommandId = request.CommandId,
				Content = $"Error handling command '{request.Command}': {ex.Message}",
				IsError = true
			} );
		}
	}

	private static CommandResponse HandleCreateComponent( CommandRequest request )
	{
		if ( !request.TryGetArgument( "componentType", out string? componentType ) || string.IsNullOrEmpty( componentType ) )
		{
			return new CommandResponse()
			{
				CommandId = request.CommandId,
				Content = "Argument 'componentType' is required",
				IsError = true
			};
		}

		if ( !request.TryGetArgument( "gameObjectId", out string? gameObjectId ) || string.IsNullOrEmpty( gameObjectId ) )
		{
			return new CommandResponse()
			{
				CommandId = request.CommandId,
				Content = "Argument 'gameObjectId' is required",
				IsError = true
			};
		}

		if ( string.IsNullOrEmpty( gameObjectId ) )
		{
			return new CommandResponse()
			{
				CommandId = request.CommandId,
				Content = "Argument 'gameObjectId' is required",
				IsError = true
			};
		}

		var gameObject = GetGameObjectByGuid( gameObjectId );
		if ( gameObject == null )
		{
			return new CommandResponse()
			{
				CommandId = request.CommandId,
				Content = $"GameObject '{gameObjectId}' not found",
				IsError = true
			};
		}

		var componentTypeDesc = TypeLibrary.GetType( componentType );
		if ( componentTypeDesc == null )
		{
			return new CommandResponse()
			{
				CommandId = request.CommandId,
				Content = $"Component type '{componentType}' not found",
				IsError = true
			};
		}

		var component = gameObject.Components.Create( componentTypeDesc );

		if ( component == null )
		{
			return new CommandResponse()
			{
				CommandId = request.CommandId,
				Content = $"Failed to create component of type '{componentType}'",
				IsError = true
			};
		}

		return new CommandResponse()
		{
			CommandId = request.CommandId,
			Content = $"Component '{componentType}' created successfully",
			IsError = false
		};
	}

	private static CommandResponse HandleGetComponents( CommandRequest request )
	{
		if ( !request.TryGetArgument( "gameObjectId", out string? gameObjectId ) || string.IsNullOrEmpty( gameObjectId ) )
		{
			return new CommandResponse()
			{
				CommandId = request.CommandId,
				Content = "Argument 'gameObjectId' is required",
				IsError = true
			};
		}

		Log.Info( $"Game object ID: {gameObjectId}" );

		var gameObject = GetGameObjectByGuid( gameObjectId );
		if ( gameObject == null )
		{
			return new CommandResponse()
			{
				CommandId = request.CommandId,
				Content = $"GameObject not found: {gameObjectId}",
				IsError = true
			};
		}

		Log.Info( $"Game object found: {gameObject.Name}" );

		var components = gameObject.Components.GetAll()
			.Select( c => new
			{
				id = c.Id,
				type = c.GetType().Name,
				enabled = c.Enabled
			} )
			.ToArray();

		return new CommandResponse()
		{
			CommandId = request.CommandId,
			Content = JsonSerializer.Serialize( components ),
			IsError = false
		};
	}

	private static GameObject? GetGameObjectByGuid( string gameObjectId )
	{
		GameObject? gameObject = null;

		if ( Guid.TryParse( gameObjectId, out var guid ) )
		{
			gameObject = SceneEditorSession.Active.Scene?.Directory.FindByGuid( guid );
		}

		return gameObject;
	}
}