using MealMind.Modules.Training.Application;
using MealMind.Modules.Training.Domain;
using MealMind.Modules.Training.Infrastructure;
using MealMind.Shared.Abstractions.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Modules.Training.Api;

public class TrainingModule : IModule
{
    public const string BasePath = "aiChat-module";

    public string Name => "AiChat";
    public string Path => BasePath;

    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddApplication(configuration)
            .AddDomain()
            .AddInfrastructure(configuration);
    }

    public void Use(WebApplication app)
    {
    }
}