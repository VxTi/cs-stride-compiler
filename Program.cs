namespace StrideCompiler;

using Logging;
using Exceptions;

class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var project = Project.Project.FromArgs(args);
            project.Compile();
        }
        catch (CompilationException ex)
        {
            Logger.Log("\e[31mFailed to compile:\n");
            ex.Log();
        }
    }
}