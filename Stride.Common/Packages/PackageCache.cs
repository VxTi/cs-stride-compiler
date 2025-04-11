using System.Text.Json;
using Stride.Common.Logging;

namespace Stride.Common.Packages;

public class PackageCache(ProjectConfig projectConfig)
    : AbstractCache<Package>(GetPackagesFromLocalCache(projectConfig))
{
    public readonly string CacheDirectoryPath = projectConfig.PackageCacheDirPath;

    public void UpdateConfigFile()
    {
        try
        {
            if (!File.Exists(projectConfig.ConfigFilePath))
                throw new FileNotFoundException($"The file {projectConfig.ConfigFilePath} could not be found.");

            projectConfig.Packages = Items
                .Select(package => $"{package.Author}/{package.Name}@{package.Version}")
                .ToArray();

            projectConfig.UpdateConfigurationFile();
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to update config file: {ex.Message}");
        }
    }

    public List<Package> All()
    {
        return Items;
    }

    public override KeyValuePair<CacheResult, Package?> Query(string query)
    {
        var queryParts = query.Split(QuerySeparator);
        if (queryParts.Length != QueryFieldCount)
            return CacheMissResult;

        foreach (var entry in Items)
        {
            var key = PackageToQueryString(entry);
            if (query.Equals(key, StringComparison.OrdinalIgnoreCase))
                return new KeyValuePair<CacheResult, Package?>(
                    CacheResult.Hit, entry
                );

            // If package name and author are the same, we have a partial match
            if (queryParts[0].Equals(entry.Name, StringComparison.OrdinalIgnoreCase) &&
                queryParts[2].Equals(entry.Author, StringComparison.OrdinalIgnoreCase)
               )
                return new KeyValuePair<CacheResult, Package?>(
                    CacheResult.PartialHit, entry
                );
        }

        return CacheMissResult;
    }

    public override void Add(Package package)
    {
        Items.Add(package);
        HasChanges = true;
    }

    public override void Flush()
    {
        if (!HasChanges)
            return;

        UpdateLocalCache();
    }

    public void Refresh()
    {
        Items = GetPackagesFromLocalCache(projectConfig);
        UpdateLocalCache();
    }

    public override void Clear()
    {
        Items.Clear();
        UpdateLocalCache();
    }

    public override void UpdateLocalCache()
    {
        EnsureLocalPackageCacheExists(projectConfig);
        try
        {
            var path = Path.Combine(CacheDirectoryPath, Globals.PackageCacheFileName);

            if (!File.Exists(path))
                return;

            var serializedDeps = JsonSerializer.Serialize(Items, JsonSerializationContext.Instance.ListPackage);

            File.WriteAllText(path, serializedDeps);
        }
        catch (Exception exception)
        {
            throw new Exception($"Failed to update package cache: {exception.Message}");
        }
    }

    public override int Remove(string projectName)
    {
        var removed = Items.RemoveAll(package => package.Name.Equals(projectName));

        var path = Path.Combine(projectConfig.PackageCacheDirPath, projectName);

        if (Directory.Exists(path)) Directory.Delete(path, true);
        HasChanges = removed > 0;
        return removed;
    }

    private static List<Package> GetPackagesFromLocalCache(ProjectConfig projectConfig)
    {
        if (!EnsureLocalPackageCacheExists(projectConfig))
            return [];

        try
        {
            var fileContent = File.ReadAllText(projectConfig.PackageCacheFilePath);

            var deserialized =
                JsonSerializer.Deserialize<List<Package>>(fileContent,
                    JsonSerializationContext.Default.ListPackage);

            return deserialized ?? [];
        }
        catch
        {
            Logger.Warn("Failed to read package cache, attempting to recreate...");
            try
            {
                File.WriteAllText(projectConfig.PackageCacheDirPath, "[]");
            }
            catch
            {
                throw new Exception("Failed to recreate package cache");
            }
        }

        return [];
    }

    public static bool EnsureLocalPackageCacheExists(ProjectConfig projectConfig)
    {
        try
        {
            if (!Directory.Exists(projectConfig.PackageCacheDirPath))
                Directory.CreateDirectory(projectConfig.PackageCacheDirPath);

            if (File.Exists(projectConfig.PackageCacheFilePath)) return true;

            File.Create(projectConfig.PackageCacheFilePath).Close();
            File.WriteAllText(projectConfig.PackageCacheFilePath, "[]");
        }
        catch (Exception exception)
        {
            throw new Exception($"Could not create package cache: {exception.Message}");
        }

        return false;
    }

    public static string PackageToQueryString(string name, string author, string version = "", string sha256 = "")
    {
        return string.Join(QuerySeparator, [name, version, author, sha256]);
    }

    public static string PackageToQueryString(Package package)
    {
        return PackageToQueryString(
            package.Name,
            package.Author,
            package.Version,
            package.Sha256
        );
    }
}