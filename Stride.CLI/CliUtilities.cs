using Stride.Common;
using Stride.Common.Logging;

namespace Stride.CLI;

public static class CliUtilities
{
    public static void TryRunForProject(Action<Project> action)
    {
        try
        {
            var cwd = Directory.GetCurrentDirectory();
            var config = ProjectConfig.GetFromPath(cwd);
            if (config == null)
            {
                Logger.Warn("Unable to locate project configuration file.");
            }
            else
            {
                action.Invoke(Project.FromConfig(config));
            }
        }
        catch (Exception e)
        {
            Logger.Error($"Failed to acquire project in current directory: {e.Message}");
        }
    }
}