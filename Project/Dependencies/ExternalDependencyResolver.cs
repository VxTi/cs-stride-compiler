using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace StrideCompiler.Project.Dependencies;

using Exceptions;
using Logging;

public partial class ExternalDependencyResolver(Project project) : AbstractDependencyResolver(project)
{
    [GeneratedRegex(@"^(?:https?://)?(?:\w+\.)?github.com/([\w-]+)/([\w-]+)")]
    private static partial Regex GitHubUrlPattern();

    private readonly Regex _githubUrlPattern = GitHubUrlPattern();

    private readonly List<ProjectConfig> _localDependencies = GetDependenciesFromCache(project.ProjectConfig);

    private static readonly HttpClient HttpClient = new();

    private static List<ProjectConfig> GetDependenciesFromCache(ProjectConfig config)
    {
        var emptyCollection = new List<ProjectConfig>();
        try
        {
            if (!Directory.Exists(config.DependencyCacheDirPath))
            {
                Directory.CreateDirectory(config.DependencyCacheDirPath);
                return emptyCollection;
            }

            if (!File.Exists(config.DependencyCachePath))
            {
                File.Create(config.DependencyCachePath).Close();
                return emptyCollection;
            }

            var fileContent = File.ReadAllText(config.DependencyCachePath);

            return JsonSerializer.Deserialize<List<ProjectConfig>>(fileContent) ?? emptyCollection;
        }
        catch
        {
            Logger.Log(LogLevel.Warn, "Dependency cache appears to be corrupt, and will therefore be regenerated.");
            File.WriteAllText(config.DependencyCachePath, "[]");
        }

        return emptyCollection;
    }

    public override async Task Resolve(string dependency)
    {
        var match = _githubUrlPattern.Match(dependency);

        if (!match.Success || match.Groups.Count < 2)
            throw new CompilationException(
                "External dependency not supported. Currently, only GitHub dependencies are supported."
            );

        var requiresUpdate = await ResolveGitHubDependency(match.Groups[1].Value, match.Groups[2].Value);

        if (requiresUpdate)
        {
            var serializedDeps = JsonSerializer.Serialize(_localDependencies);
            Logger.Log(LogLevel.Info, $"Updating dependency cache: {serializedDeps}");
            await File.WriteAllTextAsync(Project.ProjectConfig.DependencyCachePath, serializedDeps);
        }
    }

    private async Task<bool> ResolveGitHubDependency(string githubUser, string githubProjectName)
    {
        try
        {
            var sanitizedUrl = $"https://raw.githubusercontent.com/{githubUser}/{githubProjectName}/main/project.json";
            Logger.Log(LogLevel.Info, $"Resolving external dependency: {githubProjectName} from {sanitizedUrl}");

            var response = await HttpClient.GetAsync(sanitizedUrl);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"HTTP status {response.StatusCode}");

            var body = await response.Content.ReadAsStringAsync();
            var remoteDependencyConfig = JsonSerializer.Deserialize<ProjectConfig>(body);

            var localDependency = GetLocalDependencyConfigFromCache(remoteDependencyConfig.name);

            if (!ShouldReconsumeExternalDependency(localDependency, remoteDependencyConfig))
            {
                Logger.Log(LogLevel.Info, "Skipping external dependency.");
                return false;
            }

            var hasConsumed = await ConsumeDependency(githubUser, githubProjectName);

            if (!hasConsumed) return hasConsumed;

            if (localDependency.HasValue)
                _localDependencies.RemoveAll(dependency =>
                    dependency.name.Equals(localDependency.Value.name) &&
                    dependency.version.Equals(localDependency.Value.version)
                );
            _localDependencies.Add(remoteDependencyConfig);

            return hasConsumed;
        }
        catch (Exception e)
        {
            Logger.Log(LogLevel.Warn, $"Failed to resolve GitHub dependency: {e.Message}");
        }

        return false;
    }

    private async Task<bool> ConsumeDependency(string githubUser, string projectName, string branch = "main")
    {
        try
        {
            Logger.Log(LogLevel.Info, $"Downloading dependency {projectName}...");
            var downloadUrl = $"https://github.com/{githubUser}/{projectName}/archive/refs/heads/{branch}.zip";

            Logger.Log(LogLevel.Info,
                $"Downloading dependency {projectName} to {Project.ProjectConfig.DependencyCacheDirPath}");

            var zipFilePath =
                Path.Combine(Project.ProjectConfig.DependencyCacheDirPath, $"{projectName}-compressed.zip");
            var extractPath = Path.Combine(Project.ProjectConfig.DependencyCacheDirPath, projectName);

            Console.WriteLine("Downloading repository...");
            await DownloadFileAsync(downloadUrl, zipFilePath);

            Console.WriteLine("Extracting repository...");
            ExtractZipFile(zipFilePath, extractPath);

            Console.WriteLine($"{projectName} downloaded and extracted successfully.");
        }
        catch
        {
            // Failed to consume, thus no changes.
            return false;
        }

        return true;
    }

    private static async Task DownloadFileAsync(string url, string filePath)
    {
        var response = await HttpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        await using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await response.Content.CopyToAsync(fs);
    }

    private static void ExtractZipFile(string zipFilePath, string extractPath)
    {
        if (Directory.Exists(extractPath))
            Directory.Delete(extractPath, true);

        ZipFile.ExtractToDirectory(zipFilePath, extractPath);

        File.Delete(zipFilePath);
    }

    private ProjectConfig? GetLocalDependencyConfigFromCache(string? remoteProjectName)
    {
        if (string.IsNullOrEmpty(remoteProjectName))
            return null;

        foreach (var dependency in _localDependencies.Where(dependency => dependency.name.Equals(remoteProjectName)))
            return dependency;

        return null;
    }

    private static bool ShouldReconsumeExternalDependency(
        ProjectConfig? localDependency,
        ProjectConfig remoteDependencyConfig)
    {
        return !localDependency.HasValue ||
               !localDependency.Value.version.Equals(remoteDependencyConfig.version);
    }
}