using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;
using Common.Logging;

namespace Dependencies.GitHub;

public partial class GitHubDependencyResolver(ExternalDependencyResolver resolver)
{
    [GeneratedRegex(@"^(?:https?://)?(?:\w+\.)?github.com/([\w-]+)/([\w-]+)@([\w\d\.]+)")]
    private static partial Regex GitHubUrlPattern();

    private readonly Regex _githubUrlPattern = GitHubUrlPattern();

    public async Task<bool> TryResolve(string dependency)
    {
        var match = _githubUrlPattern.Match(dependency);

        Logger.Info($@"Attempting to resolve GitHub dependency: {dependency}");

        if (!match.Success)
            return false;

        if (match.Groups.Count >= 4)
            return await ResolveGitHubDependencyRelease(
                match.Groups[1].Value,
                match.Groups[2].Value,
                match.Groups[3].Value
            );

        Logger.Warn(
            "GitHub dependency must be in the format of 'github.com/<user>/<repository>@<latest | v1.0.0(-experimental)>'");
        return false;
    }

    private async Task<Release?> GetRelease(string author, string projectName, string version)
    {
        var url = $"https://api.github.com/repos/{author}/{projectName}/releases/{version}";
        return await MakeGetRequest<Release>(resolver.Http, url);
    }

    private async Task<bool> ResolveGitHubDependencyRelease(
        string author,
        string projectName,
        string version = "latest")
    {
        try
        {
            Logger.Log(LogLevel.Info, $"Resolving GitHub release: {author}@{version}");

            var release = await GetRelease(author, projectName, version);

            if (release == null)
                return false;

            var releaseSha256 = await GetReleaseSha256(author, projectName, release.Value);

            if (releaseSha256 == null)
                return false;

            if (!ShouldDownloadRelease(projectName, releaseSha256))
            {
                Logger.Log(LogLevel.Info, $"Skipping dependency {projectName}");
                return false;
            }

            var successfullyDownloaded = await DownloadGithubRelease(projectName, release.Value);

            if (!successfullyDownloaded) return false;

            resolver.RemoveDependencyFromCache(projectName);
            resolver.DependencyCache.Add(new Dependency(version, author, projectName, releaseSha256));

            return true;
        }
        catch (Exception e)
        {
            Logger.Log(LogLevel.Warn, $"Failed to resolve GitHub dependency: {e.Message}");
        }

        return false;
    }

    private async Task<string?> GetReleaseSha256(string author, string projectName, Release release)
    {
        var url = $"https://api.github.com/repos/{author}/{projectName}/git/ref/tags/{release.TagName}";
        var response = await MakeGetRequest<ReleaseTagInfo?>(resolver.Http, url);

        return response?.Object.Sha256;
    }

    /**
     * Checks whether the local cache contains the specified project.
     * If the project exists, but the project SHA256 isn't the same, then we'll re-download it.
     */
    private bool ShouldDownloadRelease(string projectName, string releaseSha256)
    {
        return resolver.DependencyCache.Any(dependency =>
            dependency.Name.Equals(projectName) && !dependency.Sha256.Equals(releaseSha256));
    }

    private async Task<bool> DownloadGithubRelease(string projectName, Release release)
    {
        try
        {
            var zipFilePath =
                Path.Combine(resolver.Project.Config.DependencyCacheDirPath, $"{projectName}-compressed.zip");

            var extractPath = Path.Combine(resolver.Project.Config.DependencyCacheDirPath, projectName);
            Logger.Info($"Downloading {projectName}...");
            await DownloadFileAsync(release.ZipUrl, zipFilePath);

            Logger.Info("Extracting...");
            ExtractZipFile(zipFilePath, extractPath);

            Logger.Info($"Extraction for {projectName} complete.");
        }
        catch
        {
            return false;
        }

        return true;
    }

    private async Task DownloadFileAsync(string url, string filePath)
    {
        var response = await resolver.Http.GetAsync(url);
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

    private static async Task<TReturn?> MakeGetRequest<TReturn>(HttpClient client, string url)
    {
        try
        {
            Logger.Info($"Request: {url}");
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/vnd.github+json");
            request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");

            var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) return JsonSerializer.Deserialize<TReturn>(body);

            Logger.Warn($"Failed to make request: HTTP status {response.StatusCode}");
        }
        catch (Exception e)
        {
            Logger.Warn($"Failed to make HTTP request: {e.Message}");
        }

        return default;
    }
}