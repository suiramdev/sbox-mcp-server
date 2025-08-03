using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SandboxModelContextProtocol.Server.Services.Models;

public class CallToolResponse
{
	[JsonIgnore]
	public string Type { get; set; } = "tool";

	[JsonPropertyName( "id" )]
	public required string Id { get; set; }

	[JsonPropertyName( "name" )]
	public required string Name { get; set; }

	[JsonPropertyName( "content" )]
	public List<JsonElement> Content { get; set; } = [];

	[JsonPropertyName( "isError" )]
	public bool IsError { get; set; }
}
