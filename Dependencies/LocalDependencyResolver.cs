using Common.Logging;
using Common.Project;

namespace Dependencies;

public class LocalDependencyResolver(Project project) : AbstractDependencyResolver(project)
{
    public override async Task Resolve(string dependency)
    {
        Logger.Log($"Resolving local dependency: {dependency}");
    }
}