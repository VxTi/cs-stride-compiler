namespace Common.Logging;

public static class Logger
{
    public static void Log(string message)
    {
        Console.WriteLine(message);
    }

    public static void Info(string message)
    {
        Log(LogLevel.Info, message);
    }
    
    public static void Warn(string message)
    {
        Log(LogLevel.Warn, message);
    }
    
    public static void Log(LogLevel level, string message)
    {
        var label = level switch
        {
            LogLevel.Debug => "DEBUG",
            LogLevel.Info => "INFO",
            LogLevel.Warn => "WARN",
            LogLevel.Error => "ERROR",
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
        };
        Log($"[{label}]: {message}");
    }
}