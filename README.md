> [!IMPORTANT]
> ## Project Discontinued
>
> Development of this library has been discontinued.
>
> I decided to stop maintaining this project as the MCP ecosystem for **s&box** started moving forward in a different direction. Around the same time, **Braxen** began developing their own MCP, and multiple other MCPs started to be released.
>
> The main issue with this library is that it required building and running an external bridge server between the coding agent and the game engine. While this approach worked, it was not easy to set up, was painful to maintain, and added unnecessary complexity for users.
>
> In the end, I moved toward a different solution: **[sdocs.suiram.dev](https://sdocs.suiram.dev)**, an MCP focused on providing agents with better context about the s&box documentation and API.
>
> This turned out to be more useful than an MCP that directly bridges with the game engine. The bigger problem was not engine communication, but the lack of reliable documentation and API context available to coding agents.
>
> This repository will remain available for reference, but it should be considered deprecated and is no longer actively developed.

# Model Context Protocol for s&box

[![.NET](https://img.shields.io/badge/.NET-9.0-blue)](https://dotnet.microsoft.com/)
[![s&box](https://img.shields.io/badge/s%26box-Compatible-orange)](https://sbox.game/)

A Model Context Protocol (MCP) server that enables AI assistants to interact with the s&box editor through real-time WebSocket communication.

This MCP server works in conjunction with the separate [s&box Adapter Library](https://github.com/suiramdev/sbox-mcp-library) to provide seamless integration between AI assistants and your s&box projects.

![](./Assets/example_1.gif)

## Prerequisites

- [Docker](https://www.docker.com/get-started) (recommended) or [.NET SDK](https://dotnet.microsoft.com/download) (for manual installation)
- [s&box](https://sbox.game/)
- An [AI assistant that supports MCP](https://docs.cursor.com/mcp/introduction) (for example, Cursor, Claude Desktop, etc.) is suggested

## Quick Start

### Step 1: Install and Run the MCP Server

To get started, make sure you have Docker installed on your system. Cursor users can simply click the button below to install and launch the MCP Server automatically, or you can follow the manual instructions below.


1. **Run the container**

    ```bash
    docker run -d -p 8080:8080 --name sbox-mcp-server ghcr.io/suiramdev/sbox-mcp-server:latest
    ```

2. **Connect your AI assistant to the running MCP Server**

    [![Install MCP Server](https://cursor.com/deeplink/mcp-install-dark.svg)](https://cursor.com/install-mcp?name=sbox-mcp-server&config=JTdCJTIydHlwZSUyMiUzQSUyMmh0dHAlMjIlMkMlMjJ1cmwlMjIlM0ElMjJodHRwJTNBJTJGJTJGbG9jYWxob3N0JTNBODA4MCUyMiU3RA%3D%3D)


<details>
<summary style="color: lightgray;">Manual Installation using .NET SDK</summary>

<br />

1. **Build the server**

    ```bash
        dotnet build
    ```

2. **Run the server**

    ```bash
        dotnet run
    ```

3. **Connect your AI assistant to the running MCP Server**

    [![Install MCP Server](https://cursor.com/deeplink/mcp-install-dark.svg)](https://cursor.com/install-mcp?name=sbox-mcp-server&config=JTdCJTIydHlwZSUyMiUzQSUyMmh0dHAlMjIlMkMlMjJ1cmwlMjIlM0ElMjJodHRwJTNBJTJGJTJGbG9jYWxob3N0JTNBODA4MCUyMiU3RA%3D%3D)

</details>

### Step 2: Install the Adapter Library in s&box

To enable interaction with s&box, you need to install the adapter library. This library allows the MCP Server to communicate with the s&box editor.

1. **Install the Adapter Library** from the [sbox-mcp-library repository](https://github.com/suiramdev/sbox-mcp-library)

2. **Follow the setup instructions** in the Adapter Library repository to:
   - Install the library in your s&box project through the Asset Library
   - Connect to this MCP Server

> [!IMPORTANT]
> The server must be running for the s&box adapter library to function. Please ensure the server is running before proceeding to the next step.

## Usage

Once both this MCP Server and the s&box Adapter Library are installed and connected, you can interact with your s&box editor using natural language through your AI assistant:

```
"Create a ModelRenderer component on the Cube object"
"Find all game objects named 'Player'"
"Set the Scale property of the Transform component on MainCamera to 2,2,2"
"Remove the Rigidbody component from the Ball object"
"Show me all components attached to the Ground object"
```

## Architecture

This MCP Server acts as a bridge between:
- **AI Assistant** ↔ **This MCP Server** ↔ **s&box Adapter Library** ↔ **s&box Editor**

The MCP Server:
- Receives commands from AI assistants via the Model Context Protocol
- Translates MCP tool calls into structured WebSocket commands
- Sends commands to the s&box Adapter Library for execution
- Returns responses back to AI assistants

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

### Related Repositories

- **This Repository**: [sbox-mcp-server](https://github.com/suiramdev/sbox-mcp-server) - The main MCP server component
- **Adapter Library**: [sbox-mcp-library](https://github.com/suiramdev/sbox-mcp-library) - s&box integration library

## Support

- **Issues**: [GitHub Issues](https://github.com/suiramdev/sbox-mcp-server/issues)
- **Discussions**: [GitHub Discussions](https://github.com/suiramdev/sbox-mcp-server/discussions)
- **Adapter Library Support**: [sbox-mcp-library Issues](https://github.com/suiramdev/sbox-mcp-library/issues)
