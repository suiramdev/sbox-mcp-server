using System;

namespace SandboxModelContextProtocol.Editor.Commands.Attributes;

/// <summary>
/// Attribute that marks a method as an MCP editor tool command.
/// The method name is automatically used as the command name unless overridden.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class McpEditorToolAttribute : Attribute
{
	/// <summary>
	/// Gets the command name. If not specified, the method name will be used.
	/// </summary>
	public string? CommandName { get; }

	/// <summary>
	/// Gets the command description.
	/// </summary>
	public string? Description { get; }

	/// <summary>
	/// Initializes a new instance of the McpEditorToolAttribute with automatic command name from method name.
	/// </summary>
	public McpEditorToolAttribute()
	{
		CommandName = null;
		Description = null;
	}

	/// <summary>
	/// Initializes a new instance of the McpEditorToolAttribute with a custom command name.
	/// </summary>
	/// <param name="commandName">The custom command name to use instead of the method name</param>
	public McpEditorToolAttribute(string commandName)
	{
		CommandName = commandName;
		Description = null;
	}

	/// <summary>
	/// Initializes a new instance of the McpEditorToolAttribute with a custom command name and description.
	/// </summary>
	/// <param name="commandName">The custom command name to use instead of the method name</param>
	/// <param name="description">The description of the command</param>
	public McpEditorToolAttribute(string commandName, string description)
	{
		CommandName = commandName;
		Description = description;
	}

	/// <summary>
	/// Gets the effective command name, using the method name if no custom name was specified.
	/// </summary>
	/// <param name="methodName">The name of the method this attribute is applied to</param>
	/// <returns>The command name to use</returns>
	public string GetCommandName(string methodName)
	{
		return CommandName ?? methodName;
	}
}