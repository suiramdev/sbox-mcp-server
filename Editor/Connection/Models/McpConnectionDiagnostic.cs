using System;

namespace SandboxModelContextProtocol.Editor.Connection.Models;

/// <summary>
/// Diagnostic information for MCP connection issues
/// </summary>
public class McpConnectionDiagnostic
{
	public enum DiagnosticType
	{
		Info,
		Warning,
		Error
	}

	public DiagnosticType Type { get; set; }
	public string Message { get; set; } = "";
	public DateTime Timestamp { get; set; }
}
