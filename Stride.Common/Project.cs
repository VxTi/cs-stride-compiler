using System.Text.Json;
using Stride.Common.Packages;
using Stride.Common.Logging;

namespace Stride.Common;

public class Project
{
    public readonly PackageCache PackageCache;
    public readonly ProjectConfig ProjectConfig;

    private Project(ProjectConfig projectConfig)
    {
        ProjectConfig = projectConfig;
        PackageCache = new PackageCache(projectConfig);
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