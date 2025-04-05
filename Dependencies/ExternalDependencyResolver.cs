using System.Text.Json;
using Common.Logging;
using Common.Project;
using Dependencies.GitHub;

namespace Dependencies;

public class ExternalDependencyResolver : AbstractDependencyResolver
{
    public readonly List<Dependency> DependencyCache;
    public readonly HttpClient Http = new();

    private readonly GitHubDependencyResolver _githubResolver;

    public ExternalDependencyResolver(Project project) : base(project)
    {
        _githubResolver = new GitHubDependencyResolver(this);
        DependencyCache = GetDependenciesFromCache(project);
    }

    public override async Task Resolve(string dependency)
    {
        var dependencyChanges = await Task.WhenAll([
            _githubResolver.TryResolve(dependency)
        ]);

        // Only update cache if any of the dependency resolvers returned true
        if (!dependencyChanges.Contains(true))
            return;

        await UpdateDependencyCache();
    }

    private static bool EnsureDependencyCacheExistence(Config config)
    {
        try
        {
            if (!Directory.Exists(config.DependencyCacheDirPath))
                Directory.CreateDirectory(config.DependencyCacheDirPath);

            if (File.Exists(config.DependencyCacheFilePath)) return true;

            File.Create(config.DependencyCacheFilePath).Close();
            File.WriteAllText(config.DependencyCacheFilePath, "[]");
        }
        catch (Exception exception)
        {
            throw new Exception($"Could not create dependency cache: {exception.Message}");
        }

        return false;
    }

    private static List<Dependency> GetDependenciesFromCache(Project project)
    {
        var emptyCollection = new List<Dependency>();

        if (EnsureDependencyCacheExistence(project.Config))
            return emptyCollection;

        try
        {
            var fileContent = File.ReadAllText(project.Config.DependencyCacheFilePath);

            return JsonSerializer.Deserialize<List<Dependency>>(fileContent) ?? emptyCollection;
        }
        catch
        {
            Logger.Warn("Failed to read dependency cache, attempting to recreate...");
            try
            {
                File.WriteAllText(project.Config.DependencyCacheFilePath, "[]");
            }
            catch
            {
                throw new Exception("Failed to recreate dependency cache");
            }
        }

        return emptyCollection;
    }

    public void RemoveDependencyFromCache(string projectName)
    {
        DependencyCache.RemoveAll(dependency => dependency.Name.Equals(projectName));
        Directory.Delete(Path.Combine(Project.Config.DependencyCacheDirPath, projectName), true);
    }

    private async Task UpdateDependencyCache()
    {
        try
        {
            var serializedDeps = JsonSerializer.Serialize(DependencyCache);
            await File.WriteAllTextAsync(Project.Config.DependencyCacheFilePath, serializedDeps);
        }
        catch (Exception exception)
        {
            throw new Exception($"Failed to update dependency cache: {exception.Message}");
        }

        Logger.Log(LogLevel.Info, $"Updating dependency cache");
    }
}