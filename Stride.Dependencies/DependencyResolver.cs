using System.Text.RegularExpressions;
using Stride.Common.Logging;
using Stride.Common.Project;

namespace Stride.Dependencies;

public static partial class DependencyResolver
{
    [GeneratedRegex(@"^(?:\w+://)?((?:\w+\.)?\w+\.\w+\/)")]
    private static partial Regex UrlMatchingRegexp();

    public static async Task ResolveFor(Project project)
    {
        if (project.Config.dependencies is not { Length: > 0 }) return;

        ExternalDependencyResolver externalResolver = new(project);
        LocalDependencyResolver localResolver = new(project);

        Logger.Log("Resolving dependencies...");

        foreach (var dependency in project.Config.dependencies)
        {
            Logger.Info($"Resolving {dependency}");
            var isExternal = UrlMatchingRegexp().Match(dependency).Success;

            if (isExternal && !(project.Config.allowExternalDependencies ?? false))
                throw new Exception(
                    "Found external dependency with 'allowExternalDependencies' set to false: " + dependency +
                    "\nUse a local version, or set 'allowExternalDependencies' set to true."
                );

            AbstractDependencyResolver resolver = isExternal ? externalResolver : localResolver;
            await resolver.Resolve(dependency);
        }
    }
}