using System.Text.Json;
using System.Text.RegularExpressions;
using Stride.Common.Logging;
using ZipFile = System.IO.Compression.ZipFile;

namespace Stride.Common.Dependencies;

public partial class DependencyManager
{
    // Matches:
    // author/dependency@version | latest
    [GeneratedRegex(@"^([\w-]+)/([\w-]+)(?:@([\w\d\.\-]+))?$")]
    private static partial Regex DependencyMatchingPattern();

    private static readonly Regex GithubUrlPattern = DependencyMatchingPattern();

    private const string LatestRelease = "latest";

    private static readonly HttpClient Http = new();

    public static void UninstallDependencies(Project project, string[] dependencies)
    {
        foreach (var dependency in dependencies)
            UninstallDependency(project, dependency);
        project.DependencyCache.Flush();
        project.DependencyCache.UpdateConfigFile();
    }

    public static void UninstallDependency(Project project, string dependency)
    {
        project.DependencyCache.Remove(dependency);
    }

    public static async Task InstallDependencies(Project project, string[] dependencies, bool forceInstall = false)
    {
        await Task.WhenAll(dependencies.Select(dependency => InstallDependency(project, dependency, forceInstall)));
        project.DependencyCache.Flush();
        project.DependencyCache.UpdateConfigFile();
    }

    public static async Task<bool> InstallDependency(Project project, string dependency, bool forceInstall = false)
    {
        return await TryResolveGitHubDependency(project.DependencyCache, dependency, forceInstall);
    }

    public static async Task<bool> TryResolveGitHubDependency(DependencyCache cache, string dependency,
        bool forceInstall = false)
    {
        var match = GithubUrlPattern.Match(dependency);

        if (!match.Success)
            return false;

        if (match.Groups.Count >= 3)
        {
            var version = match.Groups.Count > 3 && match.Groups[3].Length > 0
                ? match.Groups[3].Value : LatestRelease;

            return await ResolveGitHubDependencyRelease(
                cache, match.Groups[1].Value, match.Groups[2].Value, version, forceInstall
            );
        }

        Logger.Warn(
            "GitHub dependency must be in the format of 'github.com/<user>/<repository>@<latest | v1.0.0(-experimental)>'");
        return false;
    }

    private static async Task<Release?> GetGitHubRelease(string author, string projectName, string version)
    {
        var url = version == LatestRelease
            ? $"https://api.github.com/repos/{author}/{projectName}/releases/latest"
            : $"https://api.github.com/repos/{author}/{projectName}/releases/tags/{version}";

        return await MakeGetRequest<Release>(Http, url);
    }

    private static async Task<bool> ResolveGitHubDependencyRelease(
        DependencyCache cache,
        string author,
        string projectName,
        string version = LatestRelease,
        bool forceInstall = false
    )
    {
        try
        {
            Logger.Debug($"Resolving dependency: {author} {projectName} {version}");

            var release = await GetGitHubRelease(author, projectName, version);

            if (!release.HasValue)
                return false;

            var sha256 = await GetGitHubReleaseSha256(author, projectName, release.Value);

            if (sha256 == null)
                return false;

            if (!ShouldDownloadRelease(cache, projectName, author, version, sha256))
            {
                if (!forceInstall)
                {
                    Logger.Debug($"Skipping dependency {projectName}");
                    return false;
                }

                cache.Remove(projectName);
            }

            var successfullyDownloaded = await DownloadGithubRelease(cache, projectName, release.Value);

            if (!successfullyDownloaded)
            {
                Logger.Warn("Failed to download release");
                return false;
            }

            cache.Remove(projectName);
            cache.Add(new()
            {
                Version = version,
                Author = author,
                Name = projectName,
                Sha256 = sha256
            });

            return true;
        }
        catch (Exception e)
        {
            Logger.Warn($"Failed to resolve dependency: {e.Message}");
        }

        return false;
    }

    private static async Task<string?> GetGitHubReleaseSha256(string author, string projectName, Release release)
    {
        var url = $"https://api.github.com/repos/{author}/{projectName}/git/ref/tags/{release.TagName}";
        var response = await MakeGetRequest<ReleaseTagInfo?>(Http, url);

        return response?.Object.Sha256;
    }

    private static bool ShouldDownloadRelease(
        DependencyCache cache,
        string projectName,
        string author,
        string version,
        string releaseSha256
    )
    {
        var query = DependencyCache.DependencyToQueryString(projectName, author, version, releaseSha256);
        var queryResult = cache.Query(query);

        return queryResult.Key != CacheResult.Hit;
    }

    private static async Task<bool> DownloadGithubRelease(DependencyCache cache, string projectName, Release release)
    {
        try
        {
            var zipFilePath = Path.Combine(cache.CacheDirectoryPath, $"{projectName}-compressed.zip");
            var extractionPath = Path.Combine(cache.CacheDirectoryPath, projectName);

            Logger.Info($"Downloading {projectName}@{release.TagName} to {extractionPath}");
            await DownloadGitHubFile(release.ZipUrl, zipFilePath);
            ExtractZipFile(zipFilePath, extractionPath);

            Logger.Info($"Download for {projectName} completed.");
        }
        catch (Exception e)
        {
            Logger.Error($"Failed to download dependency: {e.Message}");
            return false;
        }

        return true;
    }

    private static async Task DownloadGitHubFile(string url, string filePath)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Accept", "application/vnd.github+json");
        request.Headers.Add("User-Agent", "Stride-Stride.Compiler");

        var response = await Http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        await using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await response.Content.CopyToAsync(fs);
        await fs.FlushAsync();
    }

    private static void ExtractZipFile(string zipFilePath, string extractPath)
    {
        if (Directory.Exists(extractPath))
            Directory.Delete(extractPath, true);

        Logger.Info($"Extracting {zipFilePath} to {extractPath}");

        ZipFile.ExtractToDirectory(zipFilePath, extractPath);

        File.Delete(zipFilePath);
    }

    private static async Task<TReturn?> MakeGetRequest<TReturn>(HttpClient client, string url)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/vnd.github+json");
            request.Headers.Add("User-Agent", "Stride-Stride.Compiler");

            var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) return JsonSerializer.Deserialize<TReturn>(body);

            Logger.Warn($"Failed to make request: HTTP status {response.StatusCode}, {body}");
        }
        catch (Exception e)
        {
            Logger.Warn($"Failed to make HTTP request: {e.Message}");
        }

        return default;
    }
}