using Common.Project;

namespace Dependencies;

public abstract class AbstractDependencyResolver(Project project)
{
    public readonly Project Project = project;
    
    public abstract Task Resolve(string dependency);
}