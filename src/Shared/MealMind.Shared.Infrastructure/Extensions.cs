using System.Reflection;
using System.Runtime.CompilerServices;
using MealMind.Shared.Abstractions.Modules;
using MealMind.Shared.Infrastructure.Auth;
using MealMind.Shared.Infrastructure.Postgres;
using MealMind.Shared.Infrastructure.Services;
using MealMind.Shared.Infrastructure.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("MealMind.Bootstrapper")]

namespace MealMind.Shared.Infrastructure;

internal static class Extensions
{
    private const string CorsPolicy = "cors";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IList<Assembly> assemblies, IList<IModule> modules,
        IConfiguration configuration)
    {
        var disabledModules = new List<string>();
        foreach (var (key, value) in configuration.AsEnumerable())
        {
            if (!key.Contains(":module:enabled"))
            {
                continue;
            }

            if (value != null && !bool.Parse(value))
            {
                disabledModules.Add(key.Split(":")[0]);
            }
        }

        services.AddCors(cors =>
        {
            cors.AddPolicy(CorsPolicy, x =>
            {
                x.WithOrigins("http://localhost:5001", "https://localhost:5001", "http://localhost:5002", "https://localhost:5002")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        services.ConfigureHttpJsonOptions(opt => { opt.SerializerOptions.PropertyNameCaseInsensitive = true; });

        services.AddSwagger();
        services.AddAuth(configuration);
        services.AddHostedService<AppInitializer>();
        services.AddServices(configuration);
        services.AddPostgres();
        services.AddMediatrWithFilters(assemblies);
        services.AddDecorators();

        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseCors(CorsPolicy);
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "MealMind API"); });
        return app;
    }

    public static T GetOptions<T>(this IServiceCollection services, string sectionName) where T : new()
    {
        using var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        return configuration.GetOptions<T>(sectionName);
    }

    public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : new()
    {
        var options = new T();
        configuration.GetSection(sectionName).Bind(options);
        return options;
    }

    public static string GetModuleName(this object value)
        => value?.GetType().GetModuleName() ?? string.Empty;

    public static string GetModuleName(this Type type)
    {
        if (type?.Namespace is null)
        {
            return string.Empty;
        }

        return type.Namespace.StartsWith("TaskManager.Modules.")
            ? type.Namespace.Split('.')[2].ToLowerInvariant()
            : string.Empty;
    }
}