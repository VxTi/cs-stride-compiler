using StrideCompiler.Io;

namespace StrideCompiler;

public class Project(ConfigFile fileConfig, SourceFile fileMain)
{
    private ConfigFile _fileConfig = fileConfig;
    private SourceFile _fileMain = fileMain;
    private ProjectConfig _config = _parseConfig(fileConfig);


    public static Project FromArgs(string[] args)
    {
        if (args.Length == 0)
        {
            string configFilePath =  
            return new Project();
        }
        return new Project(null, null);
    }

    private static ProjectConfig _parseConfig(ConfigFile fileConfig)
    {
        return new ProjectConfig();
    }
}