using Stride.Common.Logging;

namespace Stride.Common;

public class SafeExecutor
{
    public static T? TryExecute<T>(Func<T> task, string errorMessage)
    {
        try
        {
            return task.Invoke();
        }
        catch (Exception e)
        {
            Logger.Error($"{errorMessage}: {e.Message}");
        }

        return default;
    }

    public static T TryExecute<T>(Func<T> task, T defaultValue)
    {
        try
        {
            return task.Invoke();
        }
        catch
        {
            return defaultValue;
        }
    }

    public static async Task<T?> TryExecuteAsync<T>(Func<Task<T>> task, string errorMessage)
    {
        try
        {
            return await task.Invoke();
        }
        catch (Exception e)
        {
            Logger.Error($"{errorMessage}: {e.Message}");
        }

        return default;
    }

    public static async Task<T> TryExecuteAsync<T>(Func<Task<T>> task, T defaultValue)
    {
        try
        {
            return await task.Invoke();
        }
        catch
        {
            return defaultValue;
        }
    }
}