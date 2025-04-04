namespace StrideCompiler;

public static class Stride
{
    public const string BuildConfigFileName = "build.json";
    public const string SourceFileExtension = ".sr";
    public const string DefaultMainFile = "main" + SourceFileExtension;
    public const string DefaultExecutableFileName = "output";
    public const string DefaultOutputDirectory = "bin";
    
    public const string ArgvPrefix = "--";
    public const string ArgvProject = "project";
}