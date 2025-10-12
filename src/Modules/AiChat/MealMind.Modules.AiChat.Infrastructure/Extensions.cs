using MealMind.Modules.AiChat.Application.Abstractions;
using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Infrastructure.Database;
using MealMind.Modules.AiChat.Infrastructure.Database.Repositories;
using MealMind.Modules.AiChat.Infrastructure.Services;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Infrastructure.Postgres;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Modules.AiChat.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddPostgres<AiChatDbContext>()
            .AddScoped<IAiChatDbContext, AiChatDbContext>()
            .AddUnitOfWork<IUnitOfWork, UnitOfWork>()
            .AddScoped<IAiChatService, AiChatService>()
            .AddScoped<IEmbeddingService, EmbeddingService>()
            .AddScoped<IConversationRepository, ConversationRepository>();

        return services;
    }
}