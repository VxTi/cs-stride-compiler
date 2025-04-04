namespace StrideCompiler.Project.Dependencies;

using Logging;


public class ExternalDependencyResolver(Project project) : AbstractDependencyResolver(project)
{
    public override void Resolve(string dependency)
    {
        Logger.Log($"Resolving external dependency: {dependency} to {Project.Config.FullOutputPath}");
    }
}