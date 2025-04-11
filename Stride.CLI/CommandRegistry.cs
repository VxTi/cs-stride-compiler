using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Stride.Common;
using Stride.Common.Logging;

namespace Stride.CLI;

public static class CommandRegistry
{
    public static async Task<int> ProcessCommand(string[] args)
    {
        var rootCommand = new RootCommand();

        rootCommand.Description = "Commands for the compilation of a project.";
        new PackageManagementCommands().AppendCommandToParent(rootCommand);
        new CompilationCommands().AppendCommandToParent(rootCommand);
        var builder = new CommandLineBuilder(rootCommand)
            .UseDefaults()
            .UseSuggestDirective()
            .Build();

        return await builder.InvokeAsync(args);
    }
}