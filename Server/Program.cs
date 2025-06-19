using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace ModelContextProtocol.Server;

public class Program
{
	public static async Task Main(string[] args)
	{
		var builder = Host.CreateApplicationBuilder(args);

		// Configure logging to go to stderr for MCP protocol compliance
		builder.Logging.AddConsole(consoleLogOptions =>
		{
			consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
		});

		// Configure WebSocket options
		builder.Services.Configure<WebSocketOptions>(
			builder.Configuration.GetSection("WebSocket")
		);

		// Register the command service
		builder.Services.AddSingleton<ICommandService, CommandService>();

		// Configure MCP Server with stdio transport and tools from assembly
		builder.Services
			.AddMcpServer()
			.WithStdioServerTransport()
			.WithToolsFromAssembly();

		// Add WebSocket server configuration with IConfiguration
		builder.Services.AddHostedService<WebSocketServer>();

		await builder.Build().RunAsync();
	}
}
