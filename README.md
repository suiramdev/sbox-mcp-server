# Model Context Protocol for s&box

[![.NET](https://img.shields.io/badge/.NET-9.0-blue)](https://dotnet.microsoft.com/)
[![s&box](https://img.shields.io/badge/s%26box-Compatible-orange)](https://sbox.game/)

> [!IMPORTANT]
> This project is currently under active development.

A Model Context Protocol (MCP) server that enables AI assistants to interact with the s&box editor through real-time WebSocket communication.

This MCP server works in conjunction with the separate [s&box Adapter Library](https://github.com/suiramdev/sbox-mcp-library) to provide seamless integration between AI assistants and your s&box projects.

![](./Assets/example_1.gif)

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- [s&box](https://sbox.game/) (latest version)
- [s&box Adapter Library](https://github.com/suiramdev/sbox-mcp-library) (must be installed separately)
- An MCP-compatible AI assistant (Claude Desktop, Cursor, etc.)

## Quick Start

### Step 1: Install and Run the MCP Server

1. **Clone this repository**

    ```bash
    git clone https://github.com/suiramdev/sbox-mcp-server.git
    cd sbox-mcp-server
    ```

2. **Build the server**

    **Using the Build Script (Recommended):**

    ```powershell
    .\build.ps1
    ```

    **Manual Build:**

    ```bash
    dotnet build
    ```

3. **Configure your AI assistant**

    Add the MCP server to your AI assistant configuration:

    **For Cursor Editor (mcp.json):**

    ```json
    {
      "mcpServers": {
        "sbox": {
          "command": "cmd",
          "type": "stdio",
          "enable": true,
          "args": [
            "/c", 
            "<project-root>\\bin\\win-x64\\SandboxModelContextProtocol.Server.exe"
          ]
        }
      }
    }
    ```

> [!IMPORTANT]
> The server must be running for the s&box adapter library to function. Please ensure the server is running before proceeding to the next step.

### Step 2: Install the Adapter Library in s&box

Before you can interact with s&box, you must install the adapter library:

1. **Install the Adapter Library** from the [sbox-mcp-library repository](https://github.com/suiramdev/sbox-mcp-library)
2. **Follow the setup instructions** in the Adapter Library repository to:
   - Install the library in your s&box project through the Asset Library
   - Connect to this MCP Server
3. **Ensure both components are connected** before using AI assistant commands

> [!IMPORTANT]
> This MCP Server requires the separate [s&box Adapter Library](https://github.com/suiramdev/sbox-mcp-library) to communicate with the s&box editor. Please refer to the [Adapter Library documentation](https://github.com/suiramdev/sbox-mcp-library) for detailed installation and usage instructions.

## Usage

Once both this MCP Server and the s&box Adapter Library are installed and connected, you can interact with your s&box editor using natural language through your AI assistant:

```
"Create a ModelRenderer component on the Cube object"
"Find all game objects named 'Player'"
"Set the Scale property of the Transform component on MainCamera to 2,2,2"
"Remove the Rigidbody component from the Ball object"
"Show me all components attached to the Ground object"
```

## Troubleshooting

### Server Connection Issues

If the MCP Server fails to start:

1. **Verify .NET 9.0 SDK is installed** and accessible via command line
2. **Check the console output** for error messages
3. **Ensure port 8080 is available** (or configure a different port in appsettings.json)

### Testing the Server

You can manually test if the MCP Server is running by:
- Using [Postman](https://www.postman.com/downloads/) or similar API clients to send WebSocket requests to `ws://localhost:8080/ws`
- Checking the server console output for connection attempts

## Architecture

This MCP Server acts as a bridge between:
- **AI Assistant** ↔ **This MCP Server** ↔ **s&box Adapter Library** ↔ **s&box Editor**

The MCP Server:
- Receives commands from AI assistants via the Model Context Protocol
- Translates MCP tool calls into structured WebSocket commands
- Sends commands to the s&box Adapter Library for execution
- Returns responses back to AI assistants

## Configuration

The server can be configured via `appsettings.json`:

```json
{
  "WebSocket": {
    "Url": "http://localhost:8080",
    "Path": "/ws"
  }
}
```

> **Note**: Port 8080 is the recommended WebSocket port for s&box local development.

## Build Script Options

The included PowerShell build script (`build.ps1`) provides comprehensive build management:

| Command                   | Description                           |
| ------------------------- | ------------------------------------- |
| `.\build.ps1`             | Default build (Release configuration) |
| `.\build.ps1 build`       | Build in Release mode                 |
| `.\build.ps1 build-debug` | Build in Debug mode                   |
| `.\build.ps1 run`         | Run the server                        |
| `.\build.ps1 run-debug`   | Run in debug mode                     |
| `.\build.ps1 publish`     | Create self-contained executable      |
| `.\build.ps1 clean`       | Clean build artifacts                 |
| `.\build.ps1 rebuild`     | Full clean rebuild                    |
| `.\build.ps1 test`        | Run unit tests                        |
| `.\build.ps1 help`        | Show all available options            |

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

### Related Repositories

- **This Repository**: [sbox-mcp-server](https://github.com/suiramdev/sbox-mcp-server) - The main MCP server component
- **Adapter Library**: [sbox-mcp-library](https://github.com/suiramdev/sbox-mcp-library) - s&box integration library

## Support

- **Issues**: [GitHub Issues](https://github.com/suiramdev/sbox-mcp-server/issues)
- **Discussions**: [GitHub Discussions](https://github.com/suiramdev/sbox-mcp-server/discussions)
- **Adapter Library Support**: [sbox-mcp-library Issues](https://github.com/suiramdev/sbox-mcp-library/issues)
