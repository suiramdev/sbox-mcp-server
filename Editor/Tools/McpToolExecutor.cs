using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using SandboxModelContextProtocol.Editor.Commands.Attributes;
using SandboxModelContextProtocol.Editor.Tools.Models;

namespace SandboxModelContextProtocol.Editor.Tools;

public static class McpToolExecutor
{
	private static readonly Dictionary<string, MethodInfo> _toolMethods = [];
	private static bool _initialized = false;

	static McpToolExecutor()
	{
		InitializeTools();
	}

	public static async Task<CallEditorToolResponse> CallEditorTool( CallEditorToolRequest request )
	{
		try
		{
			// Ensure tools are initialized
			if ( !_initialized )
			{
				Log.Info( "Initializing tools" );
				InitializeTools();
			}

			Log.Info( $"Calling tool: {request.Name}" );
			// Find the tool method
			if ( !_toolMethods.TryGetValue( request.Name, out MethodInfo? method ) )
			{
				Log.Error( $"Tool not found: {request.Name}" );
				return new CallEditorToolResponse()
				{
					Id = request.Id,
					Name = request.Name,
					Content = [JsonSerializer.SerializeToElement( $"Tool '{request.Name}' not found" )],
					IsError = true,
				};
			}

			// Prepare arguments for method invocation
			object?[] parameters = PrepareMethodParameters( method, request.Arguments );

			Log.Info( $"Invoking method: {method.Name} with parameters: {JsonSerializer.Serialize( parameters )}" );

			// Invoke the method
			object? result = method.Invoke( null, parameters );

			// Handle async methods
			if ( result is Task task )
			{
				await task;

				// Get the result from Task<T>
				if ( task.GetType().IsGenericType )
				{
					PropertyInfo? resultProperty = task.GetType().GetProperty( "Result" );
					result = resultProperty?.GetValue( task );
				}
				else
				{
					result = null; // Task without return value
				}
			}

			// Return the result
			return new CallEditorToolResponse()
			{
				Id = request.Id,
				Name = request.Name,
				Content = [JsonSerializer.SerializeToElement( result )],
				IsError = false,
			};
		}
		catch ( Exception ex )
		{
			Log.Error( $"Error executing tool '{request.Name}': {ex.Message}\n{ex.StackTrace}" );
			Log.Error( $"Inner Exception: {ex.InnerException?.Message ?? "None"}" );
			Log.Error( $"Source: {ex.Source}" );
			Log.Error( $"Target Site: {ex.TargetSite}" );
			return new CallEditorToolResponse()
			{
				Id = request.Id,
				Name = request.Name,
				Content = [JsonSerializer.SerializeToElement( $"Error executing tool '{request.Name}': {ex.Message}" )],
				IsError = true,
			};
		}
	}

	private static void InitializeTools()
	{
		_toolMethods.Clear();

		try
		{
			// Get all assemblies in the current domain
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

			foreach ( Assembly assembly in assemblies )
			{
				try
				{
					// Find all types with McpEditorToolTypeAttribute
					Type[] toolTypes = assembly.GetTypes()
						.Where( t => t.GetCustomAttribute<McpEditorToolTypeAttribute>() != null )
						.ToArray();

					foreach ( Type toolType in toolTypes )
					{
						// Find all methods with McpEditorToolAttribute
						MethodInfo[] toolMethods = toolType.GetMethods( BindingFlags.Public | BindingFlags.Static )
							.Where( m => m.GetCustomAttribute<McpEditorToolAttribute>() != null )
							.ToArray();

						foreach ( MethodInfo method in toolMethods )
						{
							McpEditorToolAttribute? attribute = method.GetCustomAttribute<McpEditorToolAttribute>();
							if ( attribute != null )
							{
								string commandName = attribute.GetCommandName( method.Name );
								_toolMethods[commandName] = method;
							}
						}
					}
				}
				catch ( Exception ex )
				{
					// Skip assemblies that can't be reflected (e.g., native assemblies)
					Log.Error( $"Warning: Could not reflect assembly {assembly.FullName}: {ex.Message}" );
				}
			}

			_initialized = true;
		}
		catch ( Exception ex )
		{
			Log.Error( $"Error initializing MCP tools: {ex.Message}" );
			_initialized = false;
		}
	}

	private static object?[] PrepareMethodParameters( MethodInfo method, IReadOnlyDictionary<string, JsonElement>? arguments )
	{
		ParameterInfo[] parameters = method.GetParameters();
		object?[] parameterValues = new object?[parameters.Length];

		for ( int i = 0; i < parameters.Length; i++ )
		{
			ParameterInfo parameter = parameters[i];

			if ( arguments != null && arguments.TryGetValue( parameter.Name ?? string.Empty, out JsonElement argumentValue ) )
			{
				try
				{
					// Deserialize the JSON element to the parameter type
					parameterValues[i] = JsonSerializer.Deserialize( argumentValue, parameter.ParameterType );
				}
				catch ( Exception )
				{
					// If deserialization fails, try to convert the raw value
					parameterValues[i] = ConvertJsonElement( argumentValue, parameter.ParameterType );
				}
			}
			else if ( parameter.HasDefaultValue )
			{
				parameterValues[i] = parameter.DefaultValue;
			}
			else if ( parameter.ParameterType.IsValueType )
			{
				parameterValues[i] = Activator.CreateInstance( parameter.ParameterType );
			}
			else
			{
				parameterValues[i] = null;
			}
		}

		return parameterValues;
	}

	private static object? ConvertJsonElement( JsonElement element, Type targetType )
	{
		try
		{
			return element.ValueKind switch
			{
				JsonValueKind.String => element.GetString(),
				JsonValueKind.Number when targetType == typeof( int ) || targetType == typeof( int? ) => element.GetInt32(),
				JsonValueKind.Number when targetType == typeof( long ) || targetType == typeof( long? ) => element.GetInt64(),
				JsonValueKind.Number when targetType == typeof( float ) || targetType == typeof( float? ) => element.GetSingle(),
				JsonValueKind.Number when targetType == typeof( double ) || targetType == typeof( double? ) => element.GetDouble(),
				JsonValueKind.Number when targetType == typeof( decimal ) || targetType == typeof( decimal? ) => element.GetDecimal(),
				JsonValueKind.True or JsonValueKind.False when targetType == typeof( bool ) || targetType == typeof( bool? ) => element.GetBoolean(),
				JsonValueKind.Null => null,
				_ => element.GetString()
			};
		}
		catch
		{
			return targetType.IsValueType ? Activator.CreateInstance( targetType ) : null;
		}
	}
}
