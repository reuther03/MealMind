using MealMind.Modules.Identity.Application;
using MealMind.Modules.Identity.Domain;
using MealMind.Modules.Identity.Infrastructure;
using MealMind.Shared.Abstractions.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable ClassNeverInstantiated.Global

namespace MealMind.Modules.Identity.Api;

public class IdentityModule : IModule
{
    public const string BasePath = "identity-module";

    public string Name => "Identities";
    public string Path => BasePath;

    public void Register(IServiceCollection services)
    {
        services
            .AddDomain()
            .AddApplication()
            .AddInfrastructure();
    }

    public void Use(WebApplication app)
    {
    }
}