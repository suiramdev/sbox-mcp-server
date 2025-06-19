# Model Context Protocol for s&box

> This project is a work in progress, it is not fully functional yet.
> The setup process is not fully automated yet. More is to come...
> Feel free to PR, it is open source and free to use.

A Model Context Protocol (MCP) server implementation that enables AI assistants to interact with s&box game objects and components through WebSocket communication.

## Project Structure

```
ModelContextProtocolMCP/
├── Server/                    # MCP Server implementation
│   ├── Program.cs             # Main server entry point
│   ├── appsettings.json       # Server configuration
│   ├── Services/              # Core server services
│   │   ├── WebSocketServer.cs # WebSocket server for s&box communication
│   │   └── CommandService.cs  # Command execution and response handling
│   └── Tools/                 # MCP tools exposed to AI assistants
│       └── ComponentTool.cs   # s&box component management tools
├── Editor/                    # s&box Editor integration
│   ├── WebSocketClient.cs     # WebSocket client for server communication
│   ├── CommandDispatcher.cs   # Routes commands to appropriate handlers
│   └── Commands/              # Command handlers for s&box operations
```

## Server Overview

The `Server/` folder contains a standalone .NET 9.0 console application that implements the Model Context Protocol specification. It serves as a bridge between AI assistants and s&box, enabling natural language control of game development workflows.

### Key Components

- **MCP Server**: Implements the Model Context Protocol for AI assistant communication
- **WebSocket Server**: Provides real-time bidirectional communication with s&box editor
- **Command Service**: Manages command execution, response handling, and connection lifecycle

- **Component Tools**: Exposes s&box component operations as MCP tools
- ... more to come ...

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- s&box Editor

### Running the Server

1. **Navigate to the Server directory:**
   ```bash
   cd Server
   ```

2. **Build the server:**
   ```bash
   dotnet build
   ```

3. **Run the server:**
   ```bash
   dotnet run
   ```
   
   Or run the compiled executable:
   ```bash
   ./bin/win-x64/modelcontextprotocol.server.exe
   ```
   
   Or include it in your Cursor editor (mcp.json) :
   ```json
   {
    "mcpServers": {
      "sbox-mcp-server": {
        "command": "cmd",
        "type": "stdio",
        "enable": true,
        "args": [
          "/c", 
          "<path to your project folder>\\Server\\bin\\win-x64\\modelcontextprotocol.server.exe"
        ]
      }
    }
    ```   


### Configuration

The server can be configured via `appsettings.json`:

```json
{
  "WebSocket": {
    "Url": "http://localhost:8080",  // WebSocket server URL
    "Path": "/ws"                    // WebSocket endpoint path
  }
}
```

> Note: It is recommended to run it on 8080 port, because it is the supported WebSocket port for s&box on local machine.

### Connecting from s&box

1. Start the MCP server (as shown above)
2. Open your s&box project in the editor
3. Open the menu "MCP" in your tool bar and select "Connect to MCP Server"
4. You can now use the MCP tools to control your s&box project

## Architecture Overview

The system uses a dual-communication architecture:

```
AI Assistant ↔ MCP Server ↔ WebSocket ↔ s&box Editor
```

1. **AI Assistant** communicates with the MCP Server using the Model Context Protocol over stdio
2. **MCP Server** translates MCP tool calls into commands and sends them via WebSocket
3. **s&box Editor** receives commands, executes them, and sends responses back
4. **Responses** flow back through the same path to the AI assistant

## Command Flow

The command handling system is designed with a modular architecture that separates concerns:

1. **WebSocketClient** - Receives commands from MCP server and sends responses back
2. **CommandDispatcher** - Routes commands to appropriate handlers based on action type
3. **ICommandHandler** - Interface for implementing specific command handlers
4. **ComponentCommandHandler** - Handles all component-related operations

```
MCP Server → WebSocket → CommandDispatcher → ComponentCommandHandler → s&box API
                ↓
MCP Server ← WebSocket ← CommandDispatcher ← ComponentCommandHandler ← Response
```

## Supported Commands

### Component Commands

All component commands support the following actions:

- `create_component` - Creates a new component on a game object
- `get_components` - Gets all components attached to a game object
- `get_component` - Gets a specific component by type from a game object
- `remove_component` - Removes a component from a game object
- `set_component_property` - Sets a property value on a component

### Command Format

Commands are sent as JSON with the following structure:

```json
{
  "action": "create_component",
  "componentType": "ModelRenderer",
  "gameObjectId": "Cube",
  "commandId": "unique-id"
}
```

### Response Format

Responses follow this structure:

```json
{
  "commandId": "unique-id",
  "success": true,
  "data": {
    // Command-specific response data
  }
}
```

Error responses:

```json
{
  "commandId": "unique-id", 
  "success": false,
  "error": "Error message"
}
```

## GameObject Resolution

The `gameObjectId` parameter can be:

- A GUID string - Finds the object by its unique identifier
- A name string - Finds the object by name

## Development

### Adding New MCP Tools

To add new tools that AI assistants can use:

1. Create a new method in `ComponentTool.cs` or create a new tool class
2. Decorate the method with `[McpServerTool]` and `[Description]` attributes
3. The tool will be automatically discovered and exposed to AI assistants

Example:

```csharp
[McpServerTool, Description("Creates a new game object with the specified name")]
public async Task<string> CreateGameObject(string name)
{
    var command = JsonSerializer.Serialize(new
    {
        action = "create_gameobject",
        name = name
    });

    return await _commandService.ExecuteCommandAsync(command);
}
```

### Adding New Command Handlers

To add support for new command types in s&box:

1. Create a new class implementing `ICommandHandler`
2. Implement the `CanHandle(string action)` method to specify which actions it handles
3. Implement the `HandleAsync(string command)` method to process commands
4. Register the handler in `CommandDispatcher` constructor

Example:

```csharp
public class MyCommandHandler : ICommandHandler
{
    public bool CanHandle(string action)
    {
        return action == "my_custom_action";
    }

    public async Task<string> HandleAsync(string command)
    {
        // Process the command
        return CreateSuccessResponse(result, commandId);
    }
}
```

Then add it to the CommandDispatcher:

```csharp
public CommandDispatcher()
{
    _handlers = new List<ICommandHandler>
    {
        new ComponentCommandHandler(),
        new MyCommandHandler() // Add your handler here
    };
}
```

## Error Handling

All command handlers should:

- Validate input parameters
- Handle exceptions gracefully
- Return appropriate error responses
- Log errors for debugging

## Type Conversion

The `ComponentCommandHandler` includes basic type conversion for common types:

- `string`
- `int`
- `float` 
- `bool`
- `Vector3` (comma-separated values)

Additional type conversions can be added to the `ConvertValue` method as needed.

## Testing

You can test the system by:

1. **Starting the MCP server:**
   ```bash
   cd Server && dotnet run
   ```

2. **Connecting to s&box:**
   - Open s&box editor with your project
   - Open the menu "MCP" in your tool bar and select "Connect to MCP Server"
   - You can now use the MCP tools to control your s&box project

3. **Testing with an AI assistant:**
   - Configure your Postman to use the MCP server
   - Try commands like "Create a ModelRenderer component on the Cube object"

> Note: You can also use Postman to test the MCP server, it is easier to use.

Example test command via WebSocket:

```json
{
  "action": "get_components",
  "gameObjectId": "Cube"
}
```

## Troubleshooting

### Common Issues

1. **Server won't start**: Check that port 8080 is available
2. **s&box won't connect**: Verify the WebSocket URL in `appsettings.json`
3. **Commands timeout**: Check s&box console for error messages or check the MCP server console for error messages
4. **Tool not found**: Ensure the tool method has proper MCP attributes

### Logging

The server logs to stderr for MCP protocol compliance. Check the console output for detailed information about:
- WebSocket connections
- Command execution
- Error messages
- Response handlin
- g