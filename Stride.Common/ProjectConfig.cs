using System.Text.Json;
using System.Text.Json.Serialization;
using Stride.Common.Logging;

namespace Stride.Common;

[Serializable()]
public class ProjectConfig
{
    [JsonPropertyName("name")] public string Name { get; set; } = Globals.DefaultExecutableFileName;

    [JsonPropertyName("author")] public string? Author { get; set; } = Environment.UserName;

    [JsonPropertyName("version")] public string Version { get; set; } = "1.0.0";

    [JsonPropertyName("mainFile")] public string MainFileName { get; set; } = Globals.DefaultMainFile;

    [JsonPropertyName("root")] public string ProjectRootPath { get; set; } = ".";

    [JsonPropertyName("sourceRoot")] public string ProjectSourceRootDirectoryName { get; set; } = "src";


    [JsonPropertyName("externalPackages")] public bool? AllowExternalPackages { get; set; } = true;

    [JsonPropertyName("outputPath")]
    public string? OutputBuildDirectoryName { get; set; } = Globals.DefaultOutputDirectory;

    [JsonPropertyName("packages")] public string[] Packages { get; set; } = [];

    [JsonIgnore] public string OutputBuildDirectorPath => Path.Join(ProjectRootPath, OutputBuildDirectoryName);

    [JsonIgnore] public string OutputBuildFilePath => Path.Combine(OutputBuildDirectorPath, Name);

    [JsonIgnore]
    public string MainFilePath => Path.Combine(ProjectRootPath, ProjectSourceRootDirectoryName, MainFileName);

    [JsonIgnore]
    public string ConfigFilePath { get; set; } =
        Path.Combine(Directory.GetCurrentDirectory(), Globals.ProjectConfigFileName);

    [JsonIgnore]
    public string PackageCacheDirPath =>
        Path.Combine(ProjectRootPath, Globals.PackagesDirName);

    [JsonIgnore]
    public string PackageCacheFilePath =>
        Path.Combine(PackageCacheDirPath, Globals.PackageCacheFileName);

    public static ProjectConfig? GetFromPath(string path)
    {
        try
        {
            var projectConfigPath = Path.Join(path, Globals.ProjectConfigFileName);

            if (!File.Exists(projectConfigPath))
                return null;

            var fileContent = File.ReadAllText(projectConfigPath);
            var config =
                JsonSerializer.Deserialize<ProjectConfig>(fileContent, JsonSerializationContext.Default.ProjectConfig);

            if (config == null)
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

    public void UpdateConfigurationFile()
    {
        var serialized = JsonSerializer.Serialize(this, JsonSerializationContext.Instance.ProjectConfig);
        File.WriteAllText(ConfigFilePath, serialized);
    }
}