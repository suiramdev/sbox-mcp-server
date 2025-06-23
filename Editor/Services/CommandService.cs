using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SandboxModelContextProtocol.Editor.Commands;
using SandboxModelContextProtocol.Editor.Commands.Interfaces;
using SandboxModelContextProtocol.Editor.Models;

namespace SandboxModelContextProtocol.Editor.Services;

public class CommandService
{
	private readonly List<ICommandHandler> _commandHandlers;

	public CommandService()
	{
		_commandHandlers = [
			new ComponentCommandHandler(),
			new GameObjectCommandHandler()
		];
	}

	/// <summary>
	/// Executes a command and returns the response
	/// </summary>
	/// <param name="request">The command request</param>
	/// <returns>The command response</returns>
	public async Task<CommandResponse> ExecuteCommandAsync( CommandRequest request )
	{
		Log.Info( $"Executing command '{request.Command}' with arguments: {request.Arguments}" );

		var commandHandler = _commandHandlers.FirstOrDefault( handler => handler.CanHandle( request ) );

		if ( commandHandler == null )
		{
			return new CommandResponse()
			{
				CommandId = request.CommandId,
				Content = "Command not found",
				IsError = true
			};
		}

		return await commandHandler.HandleAsync( request );
	}
}