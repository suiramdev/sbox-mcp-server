namespace SandboxModelContextProtocol.Editor.Models;

public class CommandResponse
{
	public required string CommandId { get; set; }
	public required string Content { get; set; }
	public required bool IsError { get; set; }
}