using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace MealMind.Shared.Infrastructure.Modules;

public static class Extensions
{
    public static WebApplicationBuilder ConfigureModules(this WebApplicationBuilder builder)
    {
        var env = builder.Environment;
        ConfigureAppConfiguration(env, builder.Configuration);
        return builder;
    }

    private static void ConfigureAppConfiguration(IHostEnvironment env, IConfigurationBuilder config)
    {
        var settings = GetSettings("*", env);
        foreach (var setting in settings)
        {
            config.AddJsonFile(setting);
        }

        settings = GetSettings($"*.{env.EnvironmentName}", env);
        foreach (var setting in settings)
        {
            config.AddJsonFile(setting);
        }

        IEnumerable<string> GetSettings(string pattern, IHostEnvironment hostEnvironment)
            => Directory.EnumerateFiles(hostEnvironment.ContentRootPath, $"module.{pattern}.json", SearchOption.AllDirectories);
    }
}