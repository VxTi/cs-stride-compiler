namespace StrideCompiler.Project.Dependencies;

using Logging;

public class LocalDependencyResolver(Project project) : AbstractDependencyResolver(project)
{
    public override void Resolve(string dependency)
    {
        Logger.Log($"Resolving local dependency: {dependency}");
    }
}