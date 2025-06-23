using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SandboxModelContextProtocol.Editor.Commands;
using SandboxModelContextProtocol.Editor.Commands.Interfaces;
using SandboxModelContextProtocol.Editor.Models;

namespace SandboxModelContextProtocol.Editor.Services;

public sealed class CommandService
{
	private static readonly Lazy<CommandService> _instance = new( () => new CommandService() );

	public static CommandService Instance => _instance.Value;

	private readonly List<ICommandHandler> _commandHandlers;

	public CommandService()
	{
		_commandHandlers = [
			new ComponentCommandHandler()
		];
	}

	/// <summary>
	/// Executes a command and returns the response
	/// </summary>
	/// <param name="request">The command request</param>
	/// <returns>The command response</returns>
	public async Task<CommandResponse> ExecuteCommandAsync( CommandRequest request )
	{
		Log.Info( $"Processing command: {request.Command}" );

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