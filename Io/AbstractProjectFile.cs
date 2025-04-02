namespace StrideCompiler.Io
{
    public enum FileVariant
    {
        Config,
        Source
    }

    public abstract class AbstractProjectFile
    {
        private const string BuildConfigFileName = "build.json";
        private const string SourceFileExtension = ".sr";

        public required string Path;
        public readonly string Name;
        public readonly FileVariant Variant;

        protected AbstractProjectFile(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileName(Path);
            Variant = GetFileVariant(path);
        }

        private FileVariant GetFileVariant(string fileName)
        {
            if (Name.Equals(BuildConfigFileName))
                return FileVariant.Config;

            if (Name.EndsWith(SourceFileExtension))
                return FileVariant.Source;

            throw new IllegalFileTypeException(fileName);
        }

        public string Content()
        {
            return File.ReadAllText(Path);
        }
    }
}