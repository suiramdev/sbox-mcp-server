using System;
using System.Linq;
using Sandbox;
using Editor;
using SandboxModelContextProtocol.Editor.Connection;
using SandboxModelContextProtocol.Editor.Connection.Models;

namespace SandboxModelContextProtocol.Editor.UI.Widgets;

internal class McpConnectionNotice : NoticeWidget
{
	private McpConnectionStatus _connectionStatus;

	public McpConnectionNotice( McpConnectionStatus connectionStatus )
	{
		Icon = "link";
		Position = 15;
		_connectionStatus = connectionStatus;
		Reset();
	}

	protected override Vector2 SizeHint()
	{
		return 1000;
	}

	/// <summary>
	/// Called when it's about to be re-used by a new connection status
	/// </summary>
	public override void Reset()
	{
		base.Reset();

		IsRunning = true;
		Tick();
		SetBodyWidget( null );
		FixedWidth = 300;
		FixedHeight = 76;
		Title = "MCP Connection";
		BorderColor = Theme.Primary;
		isErrored = false;
	}

	bool isErrored = false;

	public override bool WantsVisible
	{
		get
		{
			if ( EditorPreferences.CompileNotifications == EditorPreferences.NotificationLevel.ShowAlways )
				return true;

			if ( EditorPreferences.CompileNotifications == EditorPreferences.NotificationLevel.ShowOnError )
				return isErrored;

			return false;
		}
	}

	public override void Tick()
	{
		if ( !IsRunning )
			return;

		if ( _connectionStatus == null )
			return;

		var diagnosticCount = _connectionStatus.Diagnostics?.Count ?? 0;
		var errorCount = _connectionStatus.Diagnostics?.Count( x => x.Type == McpConnectionDiagnostic.DiagnosticType.Error ) ?? 0;
		var warningCount = _connectionStatus.Diagnostics?.Count( x => x.Type == McpConnectionDiagnostic.DiagnosticType.Warning ) ?? 0;

		// Update based on connection state
		switch ( _connectionStatus.State )
		{
			case McpConnectionStatus.ConnectionState.Connecting:
				IsRunning = true;
				Subtitle = "Connecting to MCP server...";
				BorderColor = Theme.Primary;
				isErrored = false;
				break;

			case McpConnectionStatus.ConnectionState.Connected:
				IsRunning = false;
				isErrored = false;
				BorderColor = Theme.Green;
				Subtitle = $"Connected{(warningCount > 0 ? $", {warningCount} warnings" : "")}";
				NoticeManager.Remove( this, 2 );

				if ( EditorPreferences.NotificationSounds )
					EditorUtility.PlayRawSound( "sounds/editor/success.wav" );
				break;

			case McpConnectionStatus.ConnectionState.Failed:
				IsRunning = false;
				isErrored = true;
				BorderColor = Theme.Red;
				Subtitle = $"Connection failed{(errorCount > 0 ? $", {errorCount} errors" : "")}";
				NoticeManager.Remove( this, EditorPreferences.ErrorNotificationTimeout );
				AddDiagnostics();

				if ( EditorPreferences.NotificationSounds )
					EditorUtility.PlayRawSound( "sounds/editor/fail.wav" );
				break;

			case McpConnectionStatus.ConnectionState.Disconnected:
				IsRunning = false;
				isErrored = false;
				BorderColor = Theme.Yellow;
				Subtitle = $"Disconnected{(!string.IsNullOrEmpty( _connectionStatus.DisconnectReason ) ? $": {_connectionStatus.DisconnectReason}" : "")}";
				NoticeManager.Remove( this, 5 );

				if ( diagnosticCount > 0 )
					AddDiagnostics();
				break;
		}
	}

	protected override void OnPaint()
	{
		if ( !EditorPreferences.NotificationPopups ) return;

		base.OnPaint();
	}

	private void AddDiagnostics()
	{
		if ( _connectionStatus?.Diagnostics == null || _connectionStatus.Diagnostics.Count == 0 )
			return;

		var diagnostics = _connectionStatus.Diagnostics
			.OrderByDescending( x => (int)x.Type )
			.ThenByDescending( x => x.Timestamp )
			.Take( 5 )
			.ToArray();

		if ( diagnostics.Length == 0 ) return;

		var bodyWidget = new Widget( this )
		{
			Layout = Layout.Column()
		};

		foreach ( var diag in diagnostics )
		{
			bodyWidget.Layout.Add( new McpDiagnosticWidget( diag ) );
		}

		SetBodyWidget( bodyWidget );
	}

	[Event( "mcp.connection.started" )]
	public static void OnConnectionStarted()
	{
		// Find an old notice to replace or create a new one
		var notice = NoticeManager.All.OfType<McpConnectionNotice>().FirstOrDefault();
		if ( !notice.IsValid() ) notice = new McpConnectionNotice( McpConnectionManager.Status );

		notice._connectionStatus = McpConnectionManager.Status;
		notice.Reset();
	}

	[Event( "mcp.connection.success" )]
	public static void OnConnectionSuccess()
	{
		var notice = NoticeManager.All.OfType<McpConnectionNotice>().FirstOrDefault();
		if ( notice.IsValid() )
		{
			notice._connectionStatus = McpConnectionManager.Status;
			notice.Tick();
		}
	}

	[Event( "mcp.connection.failed" )]
	public static void OnConnectionFailed()
	{
		var notice = NoticeManager.All.OfType<McpConnectionNotice>().FirstOrDefault();
		if ( notice.IsValid() )
		{
			notice._connectionStatus = McpConnectionManager.Status;
			notice.Tick();
		}
	}

	[Event( "mcp.disconnected" )]
	public static void OnDisconnected()
	{
		var notice = NoticeManager.All.OfType<McpConnectionNotice>().FirstOrDefault();
		if ( notice.IsValid() )
		{
			notice._connectionStatus = McpConnectionManager.Status;
			notice.Tick();
		}
	}
}
