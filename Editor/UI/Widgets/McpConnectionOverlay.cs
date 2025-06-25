using Editor;
using Sandbox;
using SandboxModelContextProtocol.Editor.Connection;

namespace SandboxModelContextProtocol.Editor.UI.Widgets;

public class McpConnectionOverlay : WidgetWindow
{
	private static McpConnectionOverlay? _instance;

	public McpConnectionOverlay( Widget parent, string windowTitle ) : base( parent, windowTitle )
	{
		DeleteOnClose = true;
		FixedHeight = 20;
	}

	protected override void OnPaint()
	{
		var status = McpConnectionManager.Status;
		var (textContent, iconColor, iconName) = GetStatusDisplay( status );

		Paint.SetDefaultFont();
		var textSize = Paint.MeasureText( textContent );

		var iconWidth = 14f;
		var padding = 6f;
		var spacing = 4f;
		var totalWidth = padding + iconWidth + spacing + textSize.x + padding;

		Paint.SetPen( Theme.ControlBackground.Darken( 0.2f ) );
		Paint.SetBrush( Theme.ControlBackground.WithAlpha( 0.85f ) );
		Paint.DrawRect( new Rect( 0, 0, totalWidth, 20 ) );

		var left = padding;
		Paint.SetPen( iconColor );
		var iconRect = Paint.DrawIcon( LocalRect.Shrink( left, 0 ), iconName, 14, TextFlag.LeftCenter );
		left += iconRect.Width + spacing;

		Paint.SetPen( Theme.TextControl.WithAlpha( 0.5f ) );
		Paint.SetDefaultFont();
		Paint.DrawText( LocalRect.Shrink( left, 0 ), textContent, TextFlag.LeftCenter | TextFlag.SingleLine );
	}

	private (string text, Color color, string icon) GetStatusDisplay( McpConnectionStatus status )
	{
		return status.State switch
		{
			McpConnectionStatus.ConnectionState.Connected =>
				("Connected to MCP Server", Theme.Green, "link"),
			_ =>
				("MCP Server Disconnected", Theme.Red, "link_off")
		};
	}

	public static void Reset()
	{
		if ( _instance != null )
		{
			_instance.Close();
			_instance = null;
		}

		_instance = new McpConnectionOverlay( SceneOverlayWidget.Active, "MCP Connection" );
		_instance.AdjustSize();
		_instance.AlignToParent( TextFlag.LeftTop, new Vector2( 10, 10 ) );
		_instance.Show();
	}
}