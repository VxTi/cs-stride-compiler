namespace StrideCompiler.Logging;

public static class Logger
{
    public static void Log(string message)
    {
        Console.WriteLine(message);
    }

    public static void Log(LogLevel level, string message)
    {
        var label = level switch
        {
            LogLevel.Debug => "DEBUG",
            LogLevel.Info => "INFO",
            LogLevel.Warn => "WARN",
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
        };
        Log($"[{label}]:  {message}");
    }
}