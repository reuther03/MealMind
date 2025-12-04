using MealMind.Services.Outbox.Database;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Infrastructure.Postgres;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Services.Outbox;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddOutboxServices(IConfiguration configuration)
        {
            services
                .AddPostgres<OutboxDbContext>(configuration)
                .AddScoped<OutboxDbContext>()
                .AddHostedService<EventMessageProcessJob>()
                .AddScoped<IOutboxService, OutboxService>();

            return services;
        }
    }
}