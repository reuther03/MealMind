// ReSharper disable ClassNeverInstantiated.Global

using MealMind.Shared.Abstractions.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Services.AiChat;

public class AiChatModule : IModule
{
    public const string BasePath = "AiChat-module";

    public string Name => "AiChat";
    public string Path => BasePath;

    public void Register(IServiceCollection services)
    {
        services.AddAiChat();
    }

    public void Use(WebApplication app)
    {
    }
}