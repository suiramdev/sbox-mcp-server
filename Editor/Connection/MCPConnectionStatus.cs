using System;
using System.Collections.Generic;
using SandboxModelContextProtocol.Editor.Connection.Models;

namespace SandboxModelContextProtocol.Editor.Connection;

/// <summary>
/// Represents the status and diagnostic information for an MCP connection
/// </summary>
public class McpConnectionStatus
{
	public enum ConnectionState
	{
		Disconnected,
		Connecting,
		Connected,
		Failed
	}

	public ConnectionState State { get; set; } = ConnectionState.Disconnected;
	public string? LastError { get; set; }
	public string? DisconnectReason { get; set; }
	public DateTime? ConnectedAt { get; set; }
	public DateTime? LastAttemptAt { get; set; }
	public List<McpConnectionDiagnostic> Diagnostics { get; set; } = [];

	public bool IsConnecting => State == ConnectionState.Connecting;
	public bool IsConnected => State == ConnectionState.Connected;
	public bool HasError => State == ConnectionState.Failed;
	public bool IsDisconnected => State == ConnectionState.Disconnected;

	public void Reset()
	{
		State = ConnectionState.Connecting;
		LastError = null;
		DisconnectReason = null;
		ConnectedAt = null;
		LastAttemptAt = DateTime.Now;
	}

	public void SetConnected()
	{
		State = ConnectionState.Connected;
		ConnectedAt = DateTime.Now;
		LastError = null;
		DisconnectReason = null;
		Diagnostics.Clear();
	}

	public void SetFailed( string error )
	{
		State = ConnectionState.Failed;
		LastError = error;
		ConnectedAt = null;
		AddDiagnostic( McpConnectionDiagnostic.DiagnosticType.Error, error );
	}

	public void SetDisconnected( string reason )
	{
		State = ConnectionState.Disconnected;
		DisconnectReason = reason;
		ConnectedAt = null;
		AddDiagnostic( McpConnectionDiagnostic.DiagnosticType.Warning, $"Disconnected: {reason}" );
	}

	public void AddDiagnostic( McpConnectionDiagnostic.DiagnosticType type, string message )
	{
		Diagnostics.Add( new McpConnectionDiagnostic
		{
			Type = type,
			Message = message,
			Timestamp = DateTime.Now
		} );

		// Keep only the last 5 diagnostics
		if ( Diagnostics.Count > 5 )
		{
			Diagnostics.RemoveAt( 0 );
		}
	}
}
