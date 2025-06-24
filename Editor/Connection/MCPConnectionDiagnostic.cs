using System;

namespace SandboxModelContextProtocol.Editor.Connection;

/// <summary>
/// Diagnostic information for MCP connection issues
/// </summary>
public class MCPConnectionDiagnostic
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
