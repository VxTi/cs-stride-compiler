namespace Stride.Common;

public static class Globals
{
    public const string ProjectConfigFileName = "project.json";
    public const string SourceFileExtension = ".sr";
    public const string DefaultMainFile = "main" + SourceFileExtension;
    public const string DefaultExecutableFileName = "output";
    public const string DefaultOutputDirectory = "bin";
    public const string PackagesDirName = "packages";
    public const string PackageCacheFileName = ".package-cache.json";
    public const string DefaultMainFileContent = "use system.io;\n\ni8 main {\n    println(\"Hello world!\");\n}\n";
}