using MealMind.Modules.AiChat.Application;
using MealMind.Modules.AiChat.Domain;
using MealMind.Modules.AiChat.Infrastructure;
using MealMind.Shared.Abstractions.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Modules.AiChat.Api;

public class AiChatModule : IModule
{
    public const string BasePath = "AiChat-module";

    public string Name => "AiChat";
    public string Path => BasePath;
    
    public void Register(IServiceCollection services)
    {
        services
            .AddApplication()
            .AddDomain()
            .AddInfrastructure();
    }

    public void Use(WebApplication app)
    {
    }
}