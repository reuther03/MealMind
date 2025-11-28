using MealMind.Modules.Nutrition.Application;
using MealMind.Modules.Nutrition.Domain;
using MealMind.Modules.Nutrition.Infrastructure;
using MealMind.Shared.Abstractions.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable ClassNeverInstantiated.Global

namespace MealMind.Modules.Nutrition.Api;

public class NutritionModule : IModule
{
    public const string BasePath = "nutrition-module";

    public string Name => "Nutrition";
    public string Path => BasePath;

    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDomain()
            .AddApplication()
            .AddInfrastructure(configuration);
    }

    public void Use(WebApplication app)
    {
    }
}