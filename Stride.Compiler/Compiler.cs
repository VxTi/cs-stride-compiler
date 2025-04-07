using Stride.Common;
using Stride.Common.Logging;
using Stride.Compiler.Tokenization;

namespace Stride.Compiler;

public static class Compiler
{
    public static async Task Compile(Project project)
    {
        Logger.Log("Compiling project file...");

        if (!File.Exists(project.ProjectConfig.MainFilePath))
            throw new Exception($"Main file not found: {project.ProjectConfig.MainFilePath}");

        var tokenSet = Tokenizer.StartTokenization(project);

        foreach (var token in tokenSet.SelectMany(set => set.Tokens))
        {
            Logger.Log(token.ToString());
        }
    }

    public static async Task Compile(string[] filePaths, string[] dependencyNames)
    {
        foreach (var filePath in filePaths)
        {
            Logger.Log($"Compiling file {filePath}");
        }
    }
}