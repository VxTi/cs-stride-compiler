using System.Text.Json;
using Stride.Common.Dependencies;
using Stride.Common.Logging;

namespace Stride.Common;

public class Project
{
    public readonly DependencyCache DependencyCache;
    public readonly ProjectConfig ProjectConfig;

    private Project(ProjectConfig projectConfig)
    {
        ProjectConfig = projectConfig;
        DependencyCache = new DependencyCache(projectConfig);
    }

    public static Project FromConfig(ProjectConfig config)
    {
        config.ProjectRootPath = Path.GetFullPath(config.ProjectRootPath);
        return new Project(config);
    }

    public static Project? FromPath(string path)
    {
        var config = ProjectConfig.GetFromPath(path);
        return config == null ? null : FromConfig(config);
    }
}