#nullable enable

using System.Collections.Generic;

namespace SandboxModelContextProtocol.Editor.Models;

public class CommandResponse
{
	public required string CommandId { get; set; }
	public required string Content { get; set; }
	public required bool IsError { get; set; }
}

public record CommandRequest( string CommandId, string Command, Dictionary<string, object>? Arguments = null );