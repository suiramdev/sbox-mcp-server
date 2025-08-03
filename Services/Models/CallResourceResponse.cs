using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SandboxModelContextProtocol.Server.Services.Models;

public class CallResourceResponse
{
	[JsonPropertyName( "type" )]
	public string Type { get; } = "resource";

	[JsonPropertyName( "id" )]
	public required string Id { get; set; }

	[JsonPropertyName( "name" )]
	public required string Name { get; set; }

	[JsonPropertyName( "content" )]
	public List<JsonElement> Content { get; set; } = [];

	[JsonPropertyName( "isError" )]
	public bool IsError { get; set; }
}
