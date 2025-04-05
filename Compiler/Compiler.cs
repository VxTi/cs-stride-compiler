using System.Text.Json;
using Common.Logging;
using Common.Project;
using Compiler.Tokenization;
using Dependencies;

namespace Compiler;

public static class Compiler
{
    public static async Task Compile(Project project)
    {
        Logger.Log("Compiling project file...");
        Logger.Log(JsonSerializer.Serialize(project.Config));

        if (!File.Exists(project.Config.MainFilePath))
            throw new Exception($"Main file not found: {project.Config.MainFilePath}");

        await DependencyResolver.ResolveFor(project);
        var tokenSet = Tokenizer.StartTokenization(project);

        foreach (var set in tokenSet)
        {
            foreach (var token in set.Tokens)
            {
                Logger.Log(token.ToString());
            }
        }
    }
}