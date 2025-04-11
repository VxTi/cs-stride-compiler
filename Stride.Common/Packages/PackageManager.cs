using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;
using Stride.Common.Logging;
using ZipFile = System.IO.Compression.ZipFile;

namespace Stride.Common.Packages;

public enum PackageInstallStatus
{
    Pending,
    Failure,
    Skipped,
    Success
}

public partial class PackageManager
{
    // Matches:
    // author/package@version | latest
    [GeneratedRegex(@"^([\w-]+)/([\w-]+)(?:@([\w\d\.\-]+))?$")]
    private static partial Regex PackageMatchingPattern();

    private static readonly Regex GithubUrlPattern = PackageMatchingPattern();

    private const string LatestRelease = "latest";

    private static readonly HttpClient Http = new();

    public static int UninstallPackages(Project project, string[] packages)
    {
        var prunedDuplicates = packages.Distinct().ToArray();
        var removed = prunedDuplicates.Sum(package => UninstallPackage(project, package));
        project.PackageCache.Flush();
        project.PackageCache.UpdateConfigFile();
        return removed;
    }

    public static int UninstallPackage(Project project, string package)
    {
        return project.PackageCache.Remove(package);
    }

    public static async Task InstallPackages(Project project, string[] packages, bool forceInstall = false)
    {
        var prunedDuplicates = packages.Distinct().ToArray();
        await Task.WhenAll(prunedDuplicates.Select(package => InstallPackage(project, package, forceInstall)));
        project.PackageCache.Flush();
        project.PackageCache.UpdateConfigFile();
    }

    public static async Task<bool> InstallPackage(Project project, string package, bool forceInstall = false)
    {
        return await TryResolveGitHubPackage(project.PackageCache, package, forceInstall);
    }

    public static async Task<bool> TryResolveGitHubPackage(PackageCache cache, string package,
        bool forceInstall = false)
    {
        var match = GithubUrlPattern.Match(package);

        if (!match.Success)
            return false;

        if (match.Groups.Count >= 3)
        {
            var version = match.Groups.Count > 3 && match.Groups[3].Length > 0
                ? match.Groups[3].Value
                : LatestRelease;

            return await ResolveGitHubPackageRelease(
                cache, match.Groups[1].Value, match.Groups[2].Value, version, forceInstall
            );
        }

        Logger.Warn(
            "GitHub package must be in the format of 'github.com/<user>/<repository>@<latest | v1.0.0(-experimental)>'");
        return false;
    }

    private static async Task<Release?> GetGitHubRelease(string author, string projectName, string version)
    {
        var url = version == LatestRelease
            ? $"https://api.github.com/repos/{author}/{projectName}/releases/latest"
            : $"https://api.github.com/repos/{author}/{projectName}/releases/tags/{version}";

        return await SafeExecutor.TryExecuteAsync(() => FetchGet<Release>(Http, url),
            "Failed to find package version");
    }

    private static async Task<bool> ResolveGitHubPackageRelease(
        PackageCache cache,
        string author,
        string projectName,
        string version = LatestRelease,
        bool forceInstall = false
    )
    {
        try
        {
            Logger.Debug($"Resolving package {author}/{projectName}@{version}");

            var release = await GetGitHubRelease(author, projectName, version);

            if (release == null)
            {
                Logger.Error($"Unable to retrieve package.");
                return false;
            }

            var sha256 = await GetGitHubReleaseSha256(author, projectName, release);

            if (sha256 == null)
                return false;

            var versionName = version == LatestRelease ? release.TagName : version;

            if (!ShouldDownloadRelease(cache, projectName, author, versionName, sha256))
            {
                if (!forceInstall)
                {
                    Logger.Info($"{author}/{projectName} is already installed.");
                    return false;
                }

                cache.Remove(projectName);
            }

            var successfullyDownloaded = await DownloadGithubRelease(cache, projectName, release);

            if (!successfullyDownloaded)
            {
                Logger.Warn("Failed to download release");
                return false;
            }

            cache.Remove(projectName);
            cache.Add(new()
            {
                Version = versionName,
                Author = author,
                Name = projectName,
                Sha256 = sha256
            });

            return true;
        }
        catch (Exception e)
        {
            Logger.Warn($"Failed to resolve package: {e.Message}");
        }

        return false;
    }

    private static async Task<string?> GetGitHubReleaseSha256(string author, string projectName, Release release)
    {
        var url = $"https://api.github.com/repos/{author}/{projectName}/git/ref/tags/{release.TagName}";
        var result = await SafeExecutor.TryExecuteAsync(() => FetchGet<ReleaseTagInfo>(Http, url),
            "Failed to acquire information from package");

        return result?.Object.Sha256;
    }

    private static bool ShouldDownloadRelease(
        PackageCache cache,
        string projectName,
        string author,
        string version,
        string releaseSha256
    )
    {
        var query = PackageCache.PackageToQueryString(projectName, author, version, releaseSha256);
        var queryResult = cache.Query(query);

        return queryResult.Key != CacheResult.Hit;
    }

    private static async Task<bool> DownloadGithubRelease(PackageCache cache, string projectName, Release release)
    {
        try
        {
            var zipFileName = string.Concat(Path.GetRandomFileName().AsSpan(0, 8), ".zip");
            var zipFilePath = Path.Combine(cache.CacheDirectoryPath, zipFileName);
            var extractionPath = Path.Combine(cache.CacheDirectoryPath, projectName);

            Logger.Info($"Downloading {projectName}@{release.TagName} to {extractionPath}");
            await DownloadGitHubFile(release.ZipUrl, zipFilePath);
            ExtractZipFile(zipFilePath, extractionPath);

            Logger.Info($"Download for {projectName} completed.");
        }
        catch (Exception e)
        {
            Logger.Error($"Failed to download package: {e.Message}");
            return false;
        }

        return true;
    }

    private static async Task DownloadGitHubFile(string url, string filePath)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        PrepareGitHubHeadersForRequest(request);

        var response = await Http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        await using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await response.Content.CopyToAsync(fs);
        await fs.FlushAsync();
    }

    private static void ExtractZipFile(string zipFilePath, string extractPath)
    {
        try
        {
            if (Directory.Exists(extractPath))
                Directory.Delete(extractPath, true);

            Logger.Info($"Extracting {zipFilePath} to {extractPath}");

            ZipFile.ExtractToDirectory(zipFilePath, extractPath);

            File.Delete(zipFilePath);
        }
        catch (Exception e)
        {
            Logger.Error($"Failed to extract package: {e.Message}");
        }
    }

    private static async Task<TReturn?> FetchGet<TReturn>(HttpClient client, string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        PrepareGitHubHeadersForRequest(request);

        var response = await client.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode) return default;


        var typeInfo = JsonSerializationContext.GetTypeInfo<TReturn>();
        return (TReturn?)JsonSerializer.Deserialize(body, typeInfo);
    }

    private static void PrepareGitHubHeadersForRequest(HttpRequestMessage request)
    {
        request.Headers.Add("Accept", "application/vnd.github+json");
        request.Headers.Add("User-Agent", "stride-compiler-cli");
    }
}