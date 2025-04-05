using Common.Logging;
using Common.Project;
using Compiler.Exceptions;

namespace Compiler;


public static class Stride
{
    public static async Task Main(string[] args)
    {
        try
        {
            var project = Project.FromArgs(args);
            await Compiler.Compile(project);
        }
        catch (CompilationException ex)
        {
            Logger.Log("\e[31mFailed to compile:\n");
            ex.Log();
        }
    }
}