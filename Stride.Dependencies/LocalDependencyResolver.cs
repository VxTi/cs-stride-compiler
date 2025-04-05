using Stride.Common.Logging;
using Stride.Common.Project;

namespace Stride.Dependencies;

public class LocalDependencyResolver(Project project) : AbstractDependencyResolver(project)
{
    public override async Task Resolve(string dependency)
    {
        Logger.Log($"Resolving local dependency: {dependency}");
    }
}