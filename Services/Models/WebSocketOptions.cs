namespace SandboxModelContextProtocol.Server.Services.Models;

public class WebSocketOptions
{
	public string Url { get; set; } = "http://localhost:8080";
	public string Path { get; set; } = "/ws";
}