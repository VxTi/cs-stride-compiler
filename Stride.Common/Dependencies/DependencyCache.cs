using System.Text.Json;
using Stride.Common.Logging;

namespace Stride.Common.Dependencies;

public class DependencyCache(ProjectConfig projectConfig)
    : AbstractCache<Dependency>(GetDependenciesFromLocalCache(projectConfig))
{
    private readonly JsonSerializerOptions _serializeOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        IndentSize = 4
    };

    public string CacheDirectoryPath = projectConfig.DependencyCacheDirPath;

    public void UpdateConfigFile()
    {
        try
        {
            if (!File.Exists(projectConfig.ConfigFilePath))
                throw new FileNotFoundException($"The file {projectConfig.ConfigFilePath} could not be found.");

            projectConfig.Dependencies = Items
                .Select(dependency => $"{dependency.Author}/{dependency.Name}@{dependency.Version}")
                .ToArray();

            var serialized = JsonSerializer.Serialize(projectConfig, _serializeOptions);

            File.WriteAllText(projectConfig.ConfigFilePath, serialized);
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to update config file: {ex.Message}");
        }
    }

    public List<Dependency> All()
    {
        return Items;
    }

    public override KeyValuePair<CacheResult, Dependency?> Query(string query)
    {
        var queryParts = query.Split(QuerySeparator);
        if (queryParts.Length != QueryFieldCount)
            return CacheMissResult;

        foreach (var entry in Items)
        {
            var key = DependencyToQueryString(entry);
            if (query.Equals(key, StringComparison.OrdinalIgnoreCase))
                return new KeyValuePair<CacheResult, Dependency?>(
                    CacheResult.Hit, entry
                );

            // If package name and author are the same, we have a partial match
            if (queryParts[0].Equals(entry.Name, StringComparison.OrdinalIgnoreCase) &&
                queryParts[2].Equals(entry.Author, StringComparison.OrdinalIgnoreCase)
               )
                return new KeyValuePair<CacheResult, Dependency?>(
                    CacheResult.PartialHit, entry
                );
        }

        return CacheMissResult;
    }

    public override void Add(Dependency dependency)
    {
        Items.Add(dependency);
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
        Items = GetDependenciesFromLocalCache(projectConfig);
        UpdateLocalCache();
    }

    public override void Clear()
    {
        Items.Clear();
        UpdateLocalCache();
    }

    public override void UpdateLocalCache()
    {
        try
        {
            var serializedDeps = JsonSerializer.Serialize(Items, _serializeOptions);
            File.WriteAllText(Path.Combine(CacheDirectoryPath, Globals.DependencyCacheFileName), serializedDeps);
        }
        catch (Exception exception)
        {
            throw new Exception($"Failed to update dependency cache: {exception.Message}");
        }

        Logger.Log(LogLevel.Info, $"Updating dependency cache");
    }

    public override void Remove(string projectName)
    {
        Items.RemoveAll(dependency => dependency.Name.Equals(projectName));

        var path = Path.Combine(projectConfig.DependencyCacheDirPath, projectName);

        if (Directory.Exists(path)) Directory.Delete(path, true);
        HasChanges = true;
    }

    private static List<Dependency> GetDependenciesFromLocalCache(ProjectConfig projectConfig)
    {
        if (!EnsureLocalDependencyCacheExists(projectConfig))
            return [];

        try
        {
            var fileContent = File.ReadAllText(projectConfig.DependencyCacheFilePath);

            var deserialized = JsonSerializer.Deserialize<List<Dependency>>(fileContent);


            return JsonSerializer.Deserialize<List<Dependency>>(fileContent) ?? [];
        }
        catch
        {
            Logger.Warn("Failed to read dependency cache, attempting to recreate...");
            try
            {
                File.WriteAllText(projectConfig.DependencyCacheFilePath, "[]");
            }
            catch
            {
                throw new Exception("Failed to recreate dependency cache");
            }
        }

        return [];
    }

    private static bool EnsureLocalDependencyCacheExists(ProjectConfig projectConfig)
    {
        try
        {
            if (!Directory.Exists(projectConfig.DependencyCacheDirPath))
                Directory.CreateDirectory(projectConfig.DependencyCacheDirPath);

            if (File.Exists(projectConfig.DependencyCacheFilePath)) return true;

            File.Create(projectConfig.DependencyCacheFilePath).Close();
            File.WriteAllText(projectConfig.DependencyCacheFilePath, "[]");
        }
        catch (Exception exception)
        {
            throw new Exception($"Could not create dependency cache: {exception.Message}");
        }

        return false;
    }

    public static string DependencyToQueryString(string name, string author, string version = "", string sha256 = "")
    {
        return string.Join(QuerySeparator, [name, version, author, sha256]);
    }

    public static string DependencyToQueryString(Dependency dependency)
    {
        return DependencyToQueryString(
            dependency.Name,
            dependency.Author,
            dependency.Version,
            dependency.Sha256
        );
    }
}