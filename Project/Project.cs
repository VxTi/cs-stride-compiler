namespace StrideCompiler.Project;

using Dependencies;
using System.Text.Json;
using Exceptions;
using Logging;
using Tokenization;

public class Project(Config config)
{
    public readonly Config Config = config;

    public static Project FromArgs(string[] args)
    {
        var projectRootPath = GetArgv(args, Stride.ArgvProject);
        var projectConfigPath = Path.Join(
            projectRootPath ?? Directory.GetCurrentDirectory(),
            Stride.BuildConfigFileName
        );

        if (!File.Exists(projectConfigPath))
            return TryExtractProjectFromArguments(args);

        var configContent = File.ReadAllText(projectConfigPath);
        try
        {
            var config = JsonSerializer.Deserialize<Config>(configContent);
            // Ensures that the file can actually be resolved
            config.projectRoot = Path.Join(projectRootPath, config.projectRoot ?? ".");
            return new Project(config);
        }
        catch (ArgumentNullException nullEx)
        {
            throw new CompilationException(
                "Failed to decode project config file due to missing JSON: " + nullEx.Message);
        }
        catch (JsonException jsonEx)
        {
            throw new CompilationException("Failed to decode project config file due to type mismatch: " +
                                           jsonEx.Message);
        }
        catch (NotSupportedException notSupportedEx)
        {
            throw new CompilationException("Decoding JSON is not supported: " + notSupportedEx.Message);
        }
    }

    /**
     * Will try to extract all required configurations from the process arguments.
     * This method is only called when the project build config file is not provided.
     * @
     */
    private static Project TryExtractProjectFromArguments(string[] args)
    {
        var config = new Config();
        config.mainFile = GetArgv(args, "target");
        var projectRoot = GetArgv(args, "root");

        if (projectRoot == null)
            throw new CompilationException(
                $"Missing required project root argument. Please specify with \"{Stride.ArgvPrefix}root\"");

        config.projectRoot = projectRoot;
        config.allowExternalDependencies = HasFlag(args, "allow-external");
        config.executableName = GetArgv(args, "executable");
        config.outputPath = GetArgv(args, "out");
        config.dependencies = GetArgvArray(args, "dep");

        return new Project(config);
    }

    /**
     * Checks whether the next argument in the argument list can be a value of said process argument.
     * Values can not start with the argument prefix, e.g. <code>IsNextArgValue(["--lib", "--lib"], 0, "lib")</code>
     * would not suffice, since the next argument after index 0 is also a flag.
     */
    private static bool IsNextArgValue(string[] args, int index, string requiredValue)
    {
        return (index + 1 < args.Length &&
                !(
                    !args[index].StartsWith(Stride.ArgvPrefix) ||
                    args[index].Length <= Stride.ArgvPrefix.Length ||
                    !args[index][Stride.ArgvPrefix.Length..].StartsWith(requiredValue) ||
                    args[index + 1].StartsWith(Stride.ArgvPrefix))
            );
    }

    /**
     * Returns an array of process argument values.
     * These values are extracted based on the provided form, e.g.
     * <code>GetArgvArray(args, "lib")</code> with the arguments
     * <code>["--lib", "first", "--lib", "second", "--lib", "third"]</code> will return an array in the following format:
     * <code>["first", "second", "third"]</code>
     */
    private static string[] GetArgvArray(string[] args, string argv)
    {
        var argumentValues = new List<string>();

        for (var i = 0; i < args.Length; i += 2)
        {
            if (!IsNextArgValue(args, i, argv))
                continue;

            argumentValues.Add(args[i + 1]);
        }

        return argumentValues.ToArray();
    }

    private static string? GetArgv(string[] args, string argName)
    {
        for (var i = 0; i < args.Length; i += 2)
        {
            if (!IsNextArgValue(args, i, argName))
                continue;

            return args[i + 1];
        }

        return null;
    }

    private static bool HasFlag(string[] args, string flag)
    {
        ArgumentNullException.ThrowIfNull(flag);
        return args.Any(arg => arg.Equals(Stride.ArgvPrefix + flag));
    }

    public void Compile()
    {
        Logger.Log("Compiling project file...");
        Logger.Log(JsonSerializer.Serialize(Config));

        if (!File.Exists(Config.MainFilePath))
            throw new CompilationException(new FileNotFoundException("Main file not found.", Config.MainFilePath));

        DependencyResolver.ResolveFor(this);
        var tokenSet = Tokenizer.StartTokenization(this);
    }
}