using System;
using Editor;
using Sandbox;
using SandboxModelContextProtocol.Editor.Connection;

namespace SandboxModelContextProtocol.Editor;

/// <summary>
/// Widget for displaying individual MCP connection diagnostics
/// </summary>
internal class MCPDiagnosticWidget : Widget
{
	private readonly MCPConnectionDiagnostic _diagnostic;

	public string Message { get; private set; }
	public DateTime Timestamp { get; private set; }

	public MCPDiagnosticWidget( MCPConnectionDiagnostic diagnostic ) : base( null )
	{
		_diagnostic = diagnostic;
		FixedHeight = 18;
		MinimumWidth = 400;
		Cursor = CursorShape.Finger;

		Message = diagnostic.Message;
		Timestamp = diagnostic.Timestamp;
		ToolTip = $"{diagnostic.Type}: {diagnostic.Message}\n{diagnostic.Timestamp:yyyy-MM-dd HH:mm:ss}";
	}

	protected override void DoLayout()
	{
		base.DoLayout();
	}

	protected override void OnPaint()
	{
		base.OnPaint();

		Paint.Antialiasing = true;
		Paint.TextAntialiasing = true;

		var color = Theme.Red;
		if ( _diagnostic.Type == MCPConnectionDiagnostic.DiagnosticType.Warning ) color = Theme.Yellow;
		if ( _diagnostic.Type == MCPConnectionDiagnostic.DiagnosticType.Info ) color = Theme.Blue;

		var rect = LocalRect;
		var textColor = Color.Lerp( Color.White, color, 0.5f );

		if ( IsUnderMouse )
		{
			Paint.ClearPen();
			Paint.SetBrush( color.WithAlpha( 0.1f ) );
			Paint.DrawRect( rect );
		}

		var timeText = Timestamp.ToString( "HH:mm:ss" );

		rect = rect.Shrink( 4, 0 );

		Paint.SetPen( textColor.WithAlpha( 0.8f ) );
		var timeRect = Paint.DrawText( rect, timeText, TextFlag.RightCenter );

		rect.Right -= timeRect.Width;
		rect.Right -= 8;

		Paint.SetPen( textColor.WithAlpha( 0.5f ) );
		Paint.DrawText( rect.Shrink( 16, 0, 0, 0 ), $"{_diagnostic.Type}: {Message}", TextFlag.LeftCenter );

		Paint.ClearPen();
		Paint.SetBrush( color );
		Paint.DrawCircle( new Rect( 0, 16 ).Shrink( 2 ) );
	}

	protected override void OnMouseClick( MouseEvent e )
	{
		if ( e.LeftMouseButton )
		{
			// Copy diagnostic message to clipboard or show more details
			Log.Info( $"Diagnostic: {_diagnostic.Type} - {Message} at {Timestamp}" );
			e.Accepted = true;
			return;
		}

		base.OnMouseClick( e );
	}
}
