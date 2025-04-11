using System.CommandLine;
using Stride.Common.Logging;
using Stride.Compiler;

namespace Stride.CLI;

public class CompilationCommands : ICommandFactory
{
    public void AppendCommandToParent(RootCommand parent)
    {
        var cCompile = new Command("compile", "Command to compile a stride project with.");
        cCompile.AddAlias("cc");
        cCompile.AddAlias("c");

        cCompile.SetHandler(() =>
            CliUtilities.TryRunForProject(async void (project) =>
            {
                try
                {
                    Logger.Info($"Compiling project {project.ProjectConfig.Name}@{project.ProjectConfig.Version}");
                    
                    var startTime = DateTime.Now;
                    await Compiler.Compiler.CompileFile(project.ProjectConfig.MainFilePath,
                        project.ProjectConfig.Packages);
                    var dT = DateTime.Now - startTime;
                    
                    Logger.Info($"Compilation finished in {dT.Minutes}m {dT.Seconds}s");
                }
                catch (Exception e)
                {
                    Logger.Error($"An error occurred whilst compiling: {e.Message}");
                }
            }));

        parent.AddCommand(cCompile);
    }
}