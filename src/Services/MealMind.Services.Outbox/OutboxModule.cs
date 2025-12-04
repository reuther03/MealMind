using MealMind.Shared.Abstractions.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Services.Outbox;

public class OutboxModule : IModule
{
    public const string BasePath = "OutboxService";

    public string Name => "Outbox";
    public string Path => BasePath;

    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOutboxServices(configuration);
    }

    public void Use(WebApplication app)
    {
    }
}