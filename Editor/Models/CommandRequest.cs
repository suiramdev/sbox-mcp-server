using System.Collections.Generic;
using System.Text.Json;

namespace SandboxModelContextProtocol.Editor.Models;

public record CommandRequest( string CommandId, string Command, Dictionary<string, JsonElement>? Arguments = null )
{
	public static CommandRequest? FromJson( string json )
	{
		return JsonSerializer.Deserialize<CommandRequest>( json );
	}

	public bool TryGetArgument<T>( string key, out T? value )
	{
		if ( Arguments == null || !Arguments.TryGetValue( key, out var argument ) )
		{
			value = default;
			return false;
		}

		try
		{
			value = JsonSerializer.Deserialize<T>( argument );
			return true;
		}
		catch ( JsonException ex )
		{
			Log.Warning( $"Failed to deserialize argument '{key}' to type {typeof( T )}: {ex.Message}" );
			value = default;
			return false;
		}
	}
}