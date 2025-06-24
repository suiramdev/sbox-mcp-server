using ModelContextProtocol.Protocol;

namespace SandboxModelContextProtocol.Server.Services.Models;

public class CommandResponse
{
	public required string CommandId { get; set; }
	public required string Content { get; set; }
	public required bool IsError { get; set; }

	public CallToolResponse ToCallToolResponse()
	{
		return new CallToolResponse()
		{
			Content = [new Content() { Type = "text", Text = Content }],
			IsError = IsError
		};
	}
}
