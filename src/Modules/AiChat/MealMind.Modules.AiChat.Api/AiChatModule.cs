// ReSharper disable ClassNeverInstantiated.Global

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
    }

    public void Use(WebApplication app)
    {
    }
}