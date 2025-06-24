using System.Collections.Generic;

namespace SandboxModelContextProtocol.Server.Services.Models;

public class CommandRequest
{
	public string? CommandId { get; set; }
	public required string Command { get; set; }
	public Dictionary<string, object>? Arguments { get; set; }
}