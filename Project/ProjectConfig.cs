using System.Text.Json.Serialization;

namespace StrideCompiler.Project;

[Serializable()]
public struct ProjectConfig
{
    public string name { get; set; }

    public string? author { get; set; }

    public string version { get; set; }

    public string? mainFile { get; set; }

    public string? root { get; set; }

    public bool? allowExternalDependencies { get; set; }


    public string? outputPath { get; set; }

    public string[]? dependencies { get; set; }


    [JsonIgnore] public string ProjectRoot => root ?? Directory.GetCurrentDirectory();

    [JsonIgnore] public string[] Dependencies => dependencies ?? [];

    [JsonIgnore] public string MainFile => mainFile ?? Stride.DefaultMainFile;

    [JsonIgnore] public string ExecutableName => name ?? Stride.DefaultExecutableFileName;

    [JsonIgnore]
    public string OutputPath =>
        outputPath ?? Path.Join(Directory.GetCurrentDirectory(), Stride.DefaultOutputDirectory);

    [JsonIgnore] public string FullOutputPath => Path.Combine(OutputPath, ExecutableName);

    [JsonIgnore] public string MainFilePath => Path.Combine(ProjectRoot, MainFile);

    [JsonIgnore] public string DependencyCacheDirPath => Path.Combine(ProjectRoot, "dependencies");

    [JsonIgnore]
    public string DependencyCachePath => Path.Combine(DependencyCacheDirPath, Stride.DependencyCacheFileName);
}