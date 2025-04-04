using System.Text.RegularExpressions;
using StrideCompiler.Exceptions;
using StrideCompiler.Logging;

namespace StrideCompiler.Project.Dependencies;

public static partial class DependencyResolver
{
    private const string UrlMatchingPattern = @"^(\w+://)?((\w+\.)?\w+\.\w+\/)";
    
    public static async Task ResolveFor(Project project)
    {
        if (project.ProjectConfig.dependencies is not { Length: > 0 }) return;
        
        ExternalDependencyResolver externalResolver = new(project);
        LocalDependencyResolver localResolver = new(project);

        Logger.Log("Resolving dependencies...");

        foreach (var dependency in project.ProjectConfig.dependencies)
        {
            var isExternal = UrlMatchingRegexp().Match(dependency).Success;

            if (isExternal && !(project.ProjectConfig.allowExternalDependencies ?? false))
                throw new CompilationException(
                    "Found external dependency with 'allowExternalDependencies' set to false: " + dependency +
                    "\nUse a local version, or set 'allowExternalDependencies' set to true."
                );

            AbstractDependencyResolver resolver = isExternal ? externalResolver : localResolver;
            await resolver.Resolve(dependency);
        }
    }

    [GeneratedRegex(UrlMatchingPattern)]
    private static partial Regex UrlMatchingRegexp();
}