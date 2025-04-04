namespace StrideCompiler.Project.Dependencies;

public abstract class AbstractDependencyResolver(Project project)
{
    protected readonly Project Project = project;
    
    public abstract void Resolve(string dependency);
}