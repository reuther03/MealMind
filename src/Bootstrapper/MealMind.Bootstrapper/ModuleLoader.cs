using System.Reflection;
using MealMind.Shared.Abstractions.Modules;

namespace MealMind.Bootstrapper;

public static class ModuleLoader
{
    public static IList<Assembly> LoadAssemblies(IServiceCollection services, IConfiguration configuration)
    {
        const string modulesPath = "TaskManager.Modules.";
        const string servicesPath = "TaskManager.Services.";

        var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
        var location = assemblies.Where(x => !x.IsDynamic).Select(x => x.Location).ToArray();
        var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
            .Where(x => !location.Contains(x, StringComparer.InvariantCultureIgnoreCase))
            .ToList();

        var disabledModules = new List<string>();
        foreach (var file in files)
        {
            if (!file.Contains(modulesPath) && !file.Contains(servicesPath))
            {
                continue;
            }

            var moduleName = file.Split(modulesPath)[1].Split('.')[0];
            var servicesName = file.Split(servicesPath)[1].Split('.')[0];
            var enabled = configuration.GetValue<bool>($"{moduleName}:module:enabled");
            var servicesEnabled = configuration.GetValue<bool>($"{servicesName}:services:enabled");
            if (!enabled || !servicesEnabled)
            {
                disabledModules.Add(file);
            }
        }

        foreach (var disabledModule in disabledModules)
        {
            files.Remove(disabledModule);
        }

        files.ForEach(x => assemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(x))));

        return assemblies;
    }

    public static IList<IModule> LoadModules(IEnumerable<Assembly> assemblies) =>
        assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x => typeof(IModule).IsAssignableFrom(x) && !x.IsInterface)
            .OrderBy(x => x.Name)
            .Select(Activator.CreateInstance)
            .Cast<IModule>()
            .ToList();
}