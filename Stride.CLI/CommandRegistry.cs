using System.CommandLine;
using Stride.Common;
using Stride.Common.Logging;

namespace Stride.CLI;

public static class CommandRegistry
{
    public static async Task<int> ProcessCommand(string[] args)
    {
        var projectOption = new Option<FileInfo?>(["--project", "-P"], "The path to the project configuration file");

        var inputFilesOption = new Option<FileInfo[]>("--in", "A list of input files to compile")
        {
            AllowMultipleArgumentsPerToken = true
        };

        var dependenciesOption =
            new Option<string[]>(["--dependency", "-D"], "A list of dependencies to include in the project")
            {
                AllowMultipleArgumentsPerToken = true
            };

        var verboseOption =
            new Option<bool?>(["--verbose", "-V"], "Whether to show verbose logging during compilation.");

        var rootCommand = new RootCommand()
        {
            projectOption,
            inputFilesOption,
            dependenciesOption,
            verboseOption
        };

        rootCommand.Description = "Commands for the compilation of a project.";

        rootCommand.SetHandler((project, inputFiles, dependencies, verbose) =>
            {
                if (project is { DirectoryName: not null, Directory.Exists: true })
                {
                    Logger.Info("Preparing to compile project files...");
                    var projectData = Project.FromPath(project.DirectoryName);

                    if (projectData is not null)
                    {
                        Task.Run(() => Compiler.Compiler.Compile(projectData)).Wait();
                    }
                    else Logger.Error($"Unable to locate project file: {project.DirectoryName}");
                    return;
                }

                var files = inputFiles.Where(file => file.Exists).Select(file => file.DirectoryName ?? "").ToArray();

                Task.Run(() => Compiler.Compiler.Compile(files, dependencies)).Wait();
            }, projectOption, inputFilesOption, dependenciesOption, verboseOption);

        rootCommand.AddCommand(new DependencyManagementCommands().CreateCommand());

        return await rootCommand.InvokeAsync(args);
    }
}