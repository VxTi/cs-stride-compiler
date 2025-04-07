using System.Text.Json;
using System.Text.Json.Serialization;
using Stride.Common.Logging;

namespace Stride.Common;

[Serializable()]
public class ProjectConfig
{
    [JsonPropertyName("name")] public string Name { get; set; } = Globals.DefaultExecutableFileName;

    [JsonPropertyName("author")] public string? Author { get; set; } = "Unknown";

    [JsonPropertyName("version")] public string Version { get; set; } = "1.0.0";

    [JsonPropertyName("mainFile")] public string MainFileName { get; set; } = Globals.DefaultMainFile;

    [JsonPropertyName("root")] public string ProjectRootPath { get; set; } = ".";

    [JsonPropertyName("sourceRoot")] public string ProjectSourceRootDirectoryName { get; set; } = "src";


    [JsonPropertyName("externalDependencies")]
    public bool? AllowExternalDependencies { get; set; } = true;

    [JsonPropertyName("outputPath")]
    public string? OutputBuildDirectoryName { get; set; } = Globals.DefaultOutputDirectory;

    [JsonPropertyName("dependencies")] public string[] Dependencies { get; set; } = [];

    [JsonIgnore] public string OutputBuildDirectorPath => Path.Join(ProjectRootPath, OutputBuildDirectoryName);

    [JsonIgnore] public string OutputBuildFilePath => Path.Combine(OutputBuildDirectorPath, Name);

    [JsonIgnore]
    public string MainFilePath => Path.Combine(ProjectRootPath, ProjectSourceRootDirectoryName, MainFileName);

    [JsonIgnore] public string ConfigFilePath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), Globals.ProjectConfigFileName);
    
    [JsonIgnore]
    public string DependencyCacheDirPath =>
        Path.Combine(ProjectRootPath, Globals.DependencyDirName);

    [JsonIgnore]
    public string DependencyCacheFilePath =>
        Path.Combine(DependencyCacheDirPath, Globals.DependencyCacheFileName);

    public static ProjectConfig? GetFromPath(string path)
    {
        try
        {
            var projectConfigPath = Path.Join(path, Globals.ProjectConfigFileName);
            var config = File.Exists(projectConfigPath)
                ? JsonSerializer.Deserialize<ProjectConfig>(File.ReadAllText(projectConfigPath))
                : null;

            if (config is null)
                return null;

            config.ProjectRootPath = Path.Combine(path, config.ProjectRootPath);
            config.ConfigFilePath = projectConfigPath;

            return config;
        }
        catch (Exception e)
        {
            Logger.Error($"Project configuration file appears to be corrupt: ${e.Message}");
        }

        return null;
    }
}