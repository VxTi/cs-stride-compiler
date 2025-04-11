using System.CommandLine;
using System.Text.Json;
using Stride.Common;
using Stride.Common.Packages;
using Stride.Common.Logging;

namespace Stride.CLI;

public class PackageManagementCommands : ICommandFactory
{
    public void AppendCommandToParent(RootCommand parent)
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
                config.UpdateConfigurationFile();
                PackageCache.EnsureLocalPackageCacheExists(config);

                var path = Path.Combine(cwd, config.ProjectSourceRootDirectoryName);
                Directory.CreateDirectory(path);
                var mainFile = Path.Combine(cwd, config.ProjectSourceRootDirectoryName, config.MainFileName);
                File.WriteAllText(mainFile, Globals.DefaultMainFileContent);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Failed to initialize project: {ex}", "\n", "\n");
                return;
            }

            Logger.Info($"Initialized project configuration at {cwd}");
        });

        var cCleanCache = new Command("clean", "Clean the project dependencies.");
        cCleanCache.AddAlias("c");
        cCleanCache.SetHandler(() =>
            CliUtilities.TryRunForProject(project =>
            {
                project.PackageCache.Clear();
                project.PackageCache.UpdateConfigFile();
                Logger.Log(LogLevel.Info, $"Package cache has been cleaned.");
            }));

        var cCacheRefresh = new Command("refresh", "Refresh the project dependencies.");
        cCacheRefresh.SetHandler(() =>
            CliUtilities.TryRunForProject((project) =>
            {
                project.PackageCache.Refresh();
                Logger.Log(LogLevel.Info, $"Package cache has been refreshed.");
            }));

        var cCache = new Command("cache", "Cache the project dependencies.");
        cCache.AddCommand(cCleanCache);
        cCache.AddCommand(cCacheRefresh);
        parent.AddCommand(cCache);

        var cForceInstall = new Option<bool>("--force", () => false);

        var cArgDependencyName = new Argument<string[]>("dependency", () => [],
            description: "Force re-installation even if already installed"
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
        cInstall.SetHandler((dependencies, forceInstall) => CliUtilities.TryRunForProject(
            project =>
            {
                if (forceInstall)
                    Logger.Warn($"Forcefully installing dependencies. Use with caution.");
                Task.Run(() => PackageManager.InstallPackages(project, dependencies, forceInstall))
                    .Wait();
            }
        ), cArgDependencyName, cForceInstall);


        var cUninstallDependency = new Command("uninstall", "Uninstall the project dependencies.")
        {
            cArgDependencyName
        };
        cUninstallDependency.SetHandler(
            dependencies => CliUtilities.TryRunForProject(project =>
            {
                if (dependencies.Length == 0)
                {
                    Logger.Warn(
                        $"No dependencies provided. Please provide a list of dependencies to uninstall.");
                    return;
                }

                var removed = PackageManager.UninstallPackages(project, dependencies);
                var pluralized = removed == 1 ? "dependency" : "dependencies";
                Logger.Log(LogLevel.Info,
                    removed == 0 ? "No dependencies removed." : $"Removed {removed} {pluralized}");
            }), cArgDependencyName);

        var cListPackages = new Command("list", "List all installed dependencies.");
        cListPackages.SetHandler(() => CliUtilities.TryRunForProject(project =>
        {
            if (project.PackageCache.All().Count == 0)
            {
                Logger.Info("There are currently no dependencies installed in this project.");
                return;
            }

            string[] heading = ["Author", "Name", "Version"];

            var content = Array.ConvertAll<Package, string[]>(
                project.PackageCache.All().ToArray(),
                dependency =>
                [
                    dependency.Author,
                    dependency.Name,
                    dependency.Version
                ]);

            var tableData = (new[] { heading }).Concat(content).ToArray();

            Logger.Table(tableData);
        }));

        parent.AddCommand(cInitializeCache);
        parent.AddCommand(cInstall);
        parent.AddCommand(cUninstallDependency);
        parent.AddCommand(cListPackages);
    }
}