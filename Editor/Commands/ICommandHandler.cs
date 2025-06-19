using System.Threading.Tasks;

namespace ModelContextProtocol.Commands;

/// <summary>
/// Interface for handling commands received from MCP WebSocket
/// </summary>
public interface ICommandHandler
{
	/// <summary>
	/// Handles a command and returns the result
	/// </summary>
	/// <param name="command">The command data as JSON string</param>
	/// <returns>The result of the command execution</returns>
	Task<string> HandleAsync(string command);
	
	/// <summary>
	/// Determines if this handler can process the given action
	/// </summary>
	/// <param name="action">The action type from the command</param>
	/// <returns>True if this handler can process the action</returns>
	bool CanHandle(string action);
} 
