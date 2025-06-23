using System.Collections.Generic;
using System.Threading.Tasks;
using SandboxModelContextProtocol.Editor.Commands.Interfaces;
using SandboxModelContextProtocol.Editor.Models;

namespace SandboxModelContextProtocol.Editor.Commands;

public class ComponentCommandHandler : ICommandHandler
{
	private readonly HashSet<string> _supportedCommands = new()
	{
		"create_component",
		"get_components",
		"get_component",
		"remove_component",
	};

	public bool CanHandle( CommandRequest request )
	{
		return _supportedCommands.Contains( request.Command );
	}

	public Task<CommandResponse> HandleAsync( CommandRequest request )
	{
		return Task.FromResult( new CommandResponse()
		{
			CommandId = request.CommandId,
			Content = "Command processed successfully",
			IsError = false
		} );
	}
}