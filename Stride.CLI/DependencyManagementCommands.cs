using System.CommandLine;
using System.Text.Json;
using Stride.Common;
using Stride.Common.Dependencies;
using Stride.Common.Logging;

namespace Stride.CLI;

public class DependencyManagementCommands : AbstractCommandFactory
{
    public override Command CreateCommand()
    {
        var cDependency = new Command("dependency", "Manage project dependencies.");
        cDependency.AddAlias("dependencies");
        cDependency.AddAlias("dep");

        ComposeSubCommands(cDependency);

        return cDependency;
    }

    private static void ComposeSubCommands(Command parent)
    {
        var cwd = Directory.GetCurrentDirectory();

        var cInitializeCache = new Command("init", "Initialize the project dependencies.");
        cInitializeCache.SetHandler(() =>
        {
            var configPath = Path.Combine(cwd, Globals.ProjectConfigFileName);

            if (File.Exists(configPath))
            {
                Logger.Log(LogLevel.Warn, "Project config file already exists.", "\n", "\n");
                return;
            }

            ProjectConfig config = new();
            try
            {
                JsonSerializerOptions serializeOptions = new()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                var serialized = JsonSerializer.Serialize(config, serializeOptions);
                File.WriteAllText(configPath, serialized);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Failed to initialize project: {ex}", "\n", "\n");
                return;
            }

            Logger.Info($"Initialized project configuration: {cwd}");
        });

        CacheCommand(parent, cwd);


        var cForceInstall = new Option<bool>("--force", () => false);

        var cArgDependencyName = new Argument<string[]>("dependency", () => [],
            description: "Force reinstallation even if already installed"
        )
        {
            Description = "Name of the dependency",
        };
        var cInstall = new Command("i", "Install the project dependencies.")
        {
            cArgDependencyName,
            cForceInstall
        };
        cInstall.AddAlias("install");
        cInstall.SetHandler((dependencies, forceInstall) =>
            ExecuteOnProject(cwd,
                project =>
                {
                    if (forceInstall)
                        Logger.Warn($"Forcefully installing dependencies. Use with caution.");
                    Task.Run(() => DependencyManager.InstallDependencies(project, dependencies, forceInstall)).Wait();
                }
            ), cArgDependencyName,
            cForceInstall);


        var cUninstallDependency = new Command("uninstall", "Uninstall the project dependencies.")
        {
            cArgDependencyName
        };
        cUninstallDependency.SetHandler(
            dependencies =>
                ExecuteOnProject(cwd,
                    project =>
                    {
                        Task.Run(() => DependencyManager.UninstallDependencies(project, dependencies)).Wait();
                    }),
            cArgDependencyName);

        var cListDependencies = new Command("list", "List all installed dependencies.");
        cListDependencies.SetHandler(() =>
        {
            ExecuteOnProject(cwd, project =>
            {
                if (project.DependencyCache.All().Count == 0)
                {
                    Logger.Info("There are currently no dependencies installed in this project.");
                    return;
                }

                string[] heading = ["Author", "Name", "Version"];

                var content = Array.ConvertAll<Dependency, string[]>(
                    project.DependencyCache.All().ToArray(),
                    dependency => new[]
                    {
                        dependency.Author,
                        dependency.Name,
                        dependency.Version
                    });

                var tableData = (new[] { heading }).Concat(content).ToArray();

                Logger.Table(tableData);
            });
        });

        parent.AddCommand(cInitializeCache);
        parent.AddCommand(cInstall);
        parent.AddCommand(cUninstallDependency);
        parent.AddCommand(cListDependencies);
    }

    private static void CacheCommand(Command parent, string cwd)
    {
        var cCleanCache = new Command("clean", "Clean the project dependencies.");
        cCleanCache.AddAlias("c");
        cCleanCache.SetHandler(() =>
        {
            ExecuteOnProject(cwd, project =>
            {
                project.DependencyCache.Clear();
                project.DependencyCache.UpdateConfigFile();
            });
        });

        var cCacheRefresh = new Command("refresh", "Refresh the project dependencies.");
        cCacheRefresh.SetHandler(() => { ExecuteOnProject(cwd, project => project.DependencyCache.Refresh()); });

        var cCache = new Command("cache", "Cache the project dependencies.");
        cCache.AddCommand(cCleanCache);
        cCache.AddCommand(cCacheRefresh);
        parent.AddCommand(cCache);
    }

    private static void ExecuteOnProject(string cwd, Action<Project> action)
    {
        var config = ProjectConfig.GetFromPath(cwd);
        if (config == null)
        {
            Logger.Warn("Unable to locate project configuration file.");
            return;
        }

        action.Invoke(Project.FromConfig(config));
    }
}