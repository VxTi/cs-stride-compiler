namespace StrideCompiler.Project;

public struct Config
{
    public string? mainFile { get; set; }
    public string? projectRoot { get; set; }
    public bool? allowExternalDependencies { get; set; }
    public string? executableName { get; set; }
    public string? outputPath { get; set; }
    public string[]? dependencies { get; set; }

    public string ProjectRoot => projectRoot ?? Directory.GetCurrentDirectory();
    
    public string[] Dependencies => dependencies ?? [];
    
    public string MainFile => mainFile ?? Stride.DefaultMainFile;
    
    public string ExecutableName => executableName ?? Stride.DefaultExecutableFileName;

    public string OutputPath => outputPath ?? Path.Join(Directory.GetCurrentDirectory(), Stride.DefaultOutputDirectory);

    public string FullOutputPath => Path.Combine(OutputPath, ExecutableName);
    
    public string MainFilePath =>  Path.Combine(ProjectRoot, MainFile);
}
