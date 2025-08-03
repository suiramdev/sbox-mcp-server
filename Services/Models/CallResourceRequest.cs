using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SandboxModelContextProtocol.Server.Services.Models;

public class CallResourceRequest
{
	[JsonIgnore]
	public string Type { get; set; } = "resource";

	[JsonPropertyName( "id" )]
	public string Id { get; set; } = Guid.NewGuid().ToString();

	[JsonPropertyName( "name" )]
	public required string Name { get; init; }

	[JsonPropertyName( "arguments" )]
	public Dictionary<string, JsonElement> Arguments { get; set; } = new();
}
