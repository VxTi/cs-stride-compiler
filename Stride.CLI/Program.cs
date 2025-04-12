namespace Stride.CLI;

public static class Program
{
    private static async Task<int> Main(string[] args)
    { 
        return await CommandRegistry.ProcessCommand(args);
    }
}