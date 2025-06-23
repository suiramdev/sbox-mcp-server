using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Sandbox;

namespace ModelContextProtocol.Commands;

/// <summary>
/// Handles component-related commands for s&box scene objects
/// </summary>
public class ComponentCommandHandler : ICommandHandler
{
	private readonly HashSet<string> _supportedActions = new()
	{
		"create_component",
		"get_components", 
		"get_component",
		"remove_component",
		"set_component_property"
	};

	public bool CanHandle(string action)
	{
		return _supportedActions.Contains(action);
	}

	public async Task<string> HandleAsync(string command)
	{
		try
		{
			var commandData = JsonSerializer.Deserialize<Dictionary<string, object>>(command);
			
			if (!commandData.TryGetValue("action", out var actionObj) || actionObj is not JsonElement actionElement)
			{
				return CreateErrorResponse("Missing or invalid action in command");
			}

			var action = actionElement.GetString();
			var commandId = GetCommandId(commandData);

			return action switch
			{
				"create_component" => HandleCreateComponent(commandData, commandId),
				"get_components" => HandleGetComponents(commandData, commandId),
				"get_component" => HandleGetComponent(commandData, commandId),
				"remove_component" => HandleRemoveComponent(commandData, commandId),
				"set_component_property" => HandleSetComponentProperty(commandData, commandId),
				_ => CreateErrorResponse($"Unsupported action: {action}", commandId)
			};
		}
		catch (Exception ex)
		{
			Log.Error($"Error handling component command: {ex.Message}");
			return CreateErrorResponse($"Command execution failed: {ex.Message}");
		}
	}

	private string HandleCreateComponent(Dictionary<string, object> commandData, string commandId)
	{
		var componentType = GetStringValue(commandData, "componentType");
		var gameObjectId = GetStringValue(commandData, "gameObjectId");

		if (string.IsNullOrEmpty(componentType))
		{
			return CreateErrorResponse("componentType is required", commandId);
		}
		
		if (string.IsNullOrEmpty(gameObjectId))
		{
			return CreateErrorResponse("gameObjectId is required", commandId);
		}

		try
		{
			var gameObject = GetGameObject(gameObjectId);
			if (gameObject == null)
			{
				return CreateErrorResponse($"GameObject not found: {gameObjectId}", commandId);
			}

			// Create component using s&box API
			//
			var componentTypeDesc = TypeLibrary.GetType(componentType);
			if (componentTypeDesc == null)
			{
				return CreateErrorResponse($"Component type not found: {componentType}", commandId);
			}
			
			var component = gameObject.Components.Create(componentTypeDesc);
			
			if (component == null)
			{
				return CreateErrorResponse($"Failed to create component of type: {componentType}", commandId);
			}

			return CreateSuccessResponse(new
			{
				success = true,
				message = $"Component {componentType} created successfully",
				componentId = component.Id,
				gameObjectId = gameObject.Id
			}, commandId);
		}
		catch (Exception ex)
		{
			return CreateErrorResponse($"Failed to create component: {ex.Message}", commandId);
		}
	}

	private string HandleGetComponents(Dictionary<string, object> commandData, string commandId)
	{
		var gameObjectId = GetStringValue(commandData, "gameObjectId");

		if (string.IsNullOrEmpty(gameObjectId))
		{
			return CreateErrorResponse("gameObjectId is required", commandId);
		}

		try
		{
			var gameObject = GetGameObject(gameObjectId);
			if (gameObject == null)
			{
				return CreateErrorResponse($"GameObject not found: {gameObjectId}", commandId);
			}

			var components = gameObject.Components.GetAll()
				.Select(c => new
				{
					id = c.Id,
					type = c.GetType().Name,
					enabled = c.Enabled
				})
				.ToArray();

			return CreateSuccessResponse(new
			{
				success = true,
				gameObjectId = gameObject.Id,
				components = components
			}, commandId);
		}
		catch (Exception ex)
		{
			return CreateErrorResponse($"Failed to get components: {ex.Message}", commandId);
		}
	}

	private string HandleGetComponent(Dictionary<string, object> commandData, string commandId)
	{
		var componentType = GetStringValue(commandData, "componentType");
		var gameObjectId = GetStringValue(commandData, "gameObjectId");

		if (string.IsNullOrEmpty(componentType))
		{
			return CreateErrorResponse("componentType is required", commandId);
		}

		if (string.IsNullOrEmpty(gameObjectId))
		{
			return CreateErrorResponse("gameObjectId is required", commandId);
		}

		try
		{
			var gameObject = GetGameObject(gameObjectId);
			if (gameObject == null)
			{
				return CreateErrorResponse($"GameObject not found: {gameObjectId}", commandId);
			}

			var componentTypeDesc = TypeLibrary.GetType(componentType);
			if (componentTypeDesc == null)
			{
				return CreateErrorResponse($"Component type not found: {componentType}", commandId);
			}

			var component = gameObject.Components.Get(componentTypeDesc.TargetType);
			if (component == null)
			{
				return CreateErrorResponse($"Component of type {componentType} not found", commandId);
			}

			return CreateSuccessResponse(new
			{
				success = true,
				gameObjectId = gameObject.Id,
				component = new
				{
					id = component.Id,
					type = component.GetType().Name,
					enabled = component.Enabled
				}
			}, commandId);
		}
		catch (Exception ex)
		{
			return CreateErrorResponse($"Failed to get component: {ex.Message}", commandId);
		}
	}

	private string HandleRemoveComponent(Dictionary<string, object> commandData, string commandId)
	{
		var componentType = GetStringValue(commandData, "componentType");
		var gameObjectId = GetStringValue(commandData, "gameObjectId");

		if (string.IsNullOrEmpty(componentType))
		{
			return CreateErrorResponse("componentType is required", commandId);
		}

		if (string.IsNullOrEmpty(gameObjectId))
		{
			return CreateErrorResponse("gameObjectId is required", commandId);
		}

		try
		{
			var gameObject = GetGameObject(gameObjectId);
			if (gameObject == null)
			{
				return CreateErrorResponse($"GameObject not found: {gameObjectId}", commandId);
			}

			var componentTypeDesc = TypeLibrary.GetType(componentType);
			if (componentTypeDesc == null)
			{
				return CreateErrorResponse($"Component type not found: {componentType}", commandId);
			}

			var component = gameObject.Components.Get(componentTypeDesc.TargetType);
			if (component == null)
			{
				return CreateErrorResponse($"Component of type {componentType} not found", commandId);
			}

			component.Destroy();

			return CreateSuccessResponse(new
			{
				success = true,
				message = $"Component {componentType} removed successfully",
				gameObjectId = gameObject.Id
			}, commandId);
		}
		catch (Exception ex)
		{
			return CreateErrorResponse($"Failed to remove component: {ex.Message}", commandId);
		}
	}

	private string HandleSetComponentProperty(Dictionary<string, object> commandData, string commandId)
	{
		var componentType = GetStringValue(commandData, "componentType");
		var propertyName = GetStringValue(commandData, "propertyName");
		var propertyValue = GetStringValue(commandData, "propertyValue");
		var gameObjectId = GetStringValue(commandData, "gameObjectId");

		if (string.IsNullOrEmpty(componentType))
		{
			return CreateErrorResponse("componentType is required", commandId);
		}

		if (string.IsNullOrEmpty(propertyName))
		{
			return CreateErrorResponse("propertyName is required", commandId);
		}

		if (string.IsNullOrEmpty(gameObjectId))
		{
			return CreateErrorResponse("gameObjectId is required", commandId);
		}

		try
		{
			var gameObject = GetGameObject(gameObjectId);
			if (gameObject == null)
			{
				return CreateErrorResponse($"GameObject not found: {gameObjectId}", commandId);
			}

			var componentTypeDesc = TypeLibrary.GetType(componentType);
			if (componentTypeDesc == null)
			{
				return CreateErrorResponse($"Component type not found: {componentType}", commandId);
			}

			var component = gameObject.Components.Get(componentTypeDesc.TargetType);
			if (component == null)
			{
				return CreateErrorResponse($"Component of type {componentType} not found", commandId);
			}

			// Use reflection to set the property
			var componentTypeInfo = component.GetType();
			var property = componentTypeInfo.GetProperty(propertyName);
			
			if (property == null)
			{
				return CreateErrorResponse($"Property {propertyName} not found on component {componentType}", commandId);
			}

			if (!property.CanWrite)
			{
				return CreateErrorResponse($"Property {propertyName} is read-only", commandId);
			}

			// Convert string value to appropriate type
			var convertedValue = ConvertValue(propertyValue, property.PropertyType);
			property.SetValue(component, convertedValue);

			return CreateSuccessResponse(new
			{
				success = true,
				message = $"Property {propertyName} set to {propertyValue} on component {componentType}",
				gameObjectId = gameObject.Id,
				componentType = componentType,
				propertyName = propertyName,
				propertyValue = propertyValue
			}, commandId);
		}
		catch (Exception ex)
		{
			return CreateErrorResponse($"Failed to set component property: {ex.Message}", commandId);
		}
	}

	private GameObject GetGameObject(string gameObjectId)
	{
		// Try to find by ID first
		if (Guid.TryParse(gameObjectId, out var guid))
		{
			var gameObject = Game.ActiveScene.Directory.FindByGuid(guid);
			if (gameObject != null)
			{
				return gameObject;
			}
		}

		// If not found by ID, try to find by name
		return Game.ActiveScene.GetAllObjects(true).OfType<GameObject>().FirstOrDefault(go => go.Name == gameObjectId);
	}

	private object ConvertValue(string value, Type targetType)
	{
		if (targetType == typeof(string))
			return value;

		if (targetType == typeof(int))
			return int.Parse(value);

		if (targetType == typeof(float))
			return float.Parse(value);

		if (targetType == typeof(bool))
			return bool.Parse(value);

		if (targetType == typeof(Vector3))
		{
			var parts = value.Split(',');
			if (parts.Length == 3)
			{
				return new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
			}
		}

		// Add more type conversions as needed
		return Convert.ChangeType(value, targetType);
	}

	private string GetStringValue(Dictionary<string, object> data, string key, string defaultValue = null)
	{
		if (data.TryGetValue(key, out var value) && value is JsonElement element)
		{
			return element.GetString();
		}
		return defaultValue;
	}

	private string GetCommandId(Dictionary<string, object> commandData)
	{
		return GetStringValue(commandData, "commandId", Guid.NewGuid().ToString());
	}

	private string CreateSuccessResponse(object data, string commandId)
	{
		return JsonSerializer.Serialize(new
		{
			commandId = commandId,
			success = true,
			data = data
		});
	}

	private string CreateErrorResponse(string error, string commandId = null)
	{
		return JsonSerializer.Serialize(new
		{
			commandId = commandId ?? Guid.NewGuid().ToString(),
			success = false,
			error = error
		});
	}
} 
