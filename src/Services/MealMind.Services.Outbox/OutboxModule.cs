using MealMind.Services.Outbox.Database;
using MealMind.Shared.Abstractions.Modules;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Infrastructure.Postgres;
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
        services
            .AddPostgres<OutboxDbContext>(configuration)
            .AddScoped<OutboxDbContext>()
            .AddHostedService<EventMessageProcessJob>()
            .AddScoped<IOutboxService, OutboxService>();
    }

    public void Use(WebApplication app)
    {
    }
}