using Sandbox;
using System.Collections.Generic;
using System.Threading.Tasks;
using SandboxModelContextProtocol.Editor.Commands.Interfaces;
using SandboxModelContextProtocol.Editor.Models;
using System;
using System.Linq;
using System.Text.Json;

namespace SandboxModelContextProtocol.Editor.Commands;

public class GameObjectCommandHandler : ICommandHandler
{
	private readonly HashSet<string> _supportedCommands =
	[
		"find_game_object_by_name",
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
				"find_game_object_by_name" => Task.FromResult( HandleFindGameObjectByName( request ) ),
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

	private static CommandResponse HandleFindGameObjectByName( CommandRequest request )
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

		var gameObject = Game.ActiveScene?.Directory.FindByName( name, false );

		if ( gameObject == null )
		{
			return new CommandResponse()
			{
				CommandId = request.CommandId,
				Content = $"GameObject with name '{name}' not found",
				IsError = true
			};
		}

		return new CommandResponse()
		{
			CommandId = request.CommandId,
			Content = JsonSerializer.Serialize( gameObject.ToArray() ),
			IsError = false
		};
	}
}