// ReSharper disable ClassNeverInstantiated.Global

using MealMind.Shared.Abstractions.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Services.AiChat;

public class AIChatModule : IModule
{
    public const string BasePath = "AIChat-module";

    public string Name => "AIChat";
    public string Path => BasePath;

    public void Register(IServiceCollection services)
    {
        services.AddAiChat();
    }

    public void Use(WebApplication app)
    {
    }
}