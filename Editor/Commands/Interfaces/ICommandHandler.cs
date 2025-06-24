using System.Threading.Tasks;
using SandboxModelContextProtocol.Editor.Commands.Models;

namespace SandboxModelContextProtocol.Editor.Commands.Interfaces;

/// <summary>
/// Interface for handling commands received from MCP WebSocket
/// </summary>
public interface ICommandHandler
{
	/// <summary>
	/// Handles a command and returns the result
	/// </summary>
	/// <param name="request">The command request</param>
	/// <returns>The result of the command execution</returns>
	Task<CommandResponse> HandleAsync( CommandRequest request );

	/// <summary>
	/// Determines if this handler can process the given command request
	/// </summary>
	/// <param name="request">The command request</param>
	/// <returns>True if this handler can process the action</returns>
	bool CanHandle( CommandRequest request );
}
