namespace ModelContextProtocol;

/// <summary>
/// Re-implements the Log class from Sandbox to add a custom behavior.
/// </summary>
public static class Log
{
	private static Sandbox.Diagnostics.Logger _logger = new Sandbox.Diagnostics.Logger( "ModelContextProtocol" );

	public static void Info( string message )
	{
		_logger.Info( message );
	}

	public static void Warning( string message, params object[] args )
	{
		_logger.Warning( message );
	}

	public static void Error( string message, params object[] args )
	{
		_logger.Error( message );
	}

	public static void Trace( string message, params object[] args )
	{
		_logger.Trace( message );
	}
}