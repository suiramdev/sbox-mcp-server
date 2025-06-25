using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SandboxModelContextProtocol.Editor.Tools.Models;

public class CallEditorToolRequest
{
	[JsonPropertyName( "id" )]
	public required string Id { get; init; }

	[JsonPropertyName( "name" )]
	public required string Name { get; init; }

	[JsonPropertyName( "arguments" )]
	public IReadOnlyDictionary<string, JsonElement>? Arguments { get; init; }
}