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

public class GameObjectCommandHandler : ICommandHandler
{
	private readonly HashSet<string> _supportedCommands =
	[
		"find_game_objects_by_name",
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
				"find_game_objects_by_name" => Task.FromResult( HandleFindGameObjectsByName( request ) ),
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

	private static CommandResponse HandleFindGameObjectsByName( CommandRequest request )
	{
		if ( !request.TryGetArgument( "name", out string? name ) || string.IsNullOrEmpty( name ) )
		{
			return new CommandResponse()
			{
				CommandId = request.CommandId,
				Content = "Argument 'name' is required",
				IsError = true
			};
		}

		Scene? scene = SceneEditorSession.Active.Scene;
		if ( scene == null )
		{
			return new CommandResponse()
			{
				CommandId = request.CommandId,
				Content = "No active scene found to search for game objects",
				IsError = true
			};
		}

		var gameObjects = scene.Directory.FindByName( name, false );
		if ( !gameObjects.Any() )
		{
			return new CommandResponse()
			{
				CommandId = request.CommandId,
				Content = $"No game objects found with name '{name}'",
				IsError = true
			};
		}

		return new CommandResponse()
		{
			CommandId = request.CommandId,
			Content = JsonSerializer.Serialize( gameObjects.Select( go => go.Serialize() ).ToArray() ),
			IsError = false
		};
	}
}