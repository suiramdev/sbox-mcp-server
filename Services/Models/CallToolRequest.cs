using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SandboxModelContextProtocol.Server.Services.Models;

public class CallToolRequest
{
	[JsonPropertyName( "type" )]
	public string Type { get; } = "tool";

	[JsonPropertyName( "id" )]
	public string Id { get; set; } = Guid.NewGuid().ToString();

	[JsonPropertyName( "name" )]
	public required string Name { get; init; }

	[JsonPropertyName( "arguments" )]
	public IReadOnlyDictionary<string, JsonElement>? Arguments { get; init; }
}
