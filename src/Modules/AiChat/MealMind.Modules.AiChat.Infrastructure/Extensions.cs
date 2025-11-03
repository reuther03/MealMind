using System.ClientModel;
using MealMind.Modules.AiChat.Application.Abstractions;
using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Modules.AiChat.Application.Options;
using MealMind.Modules.AiChat.Infrastructure.Database;
using MealMind.Modules.AiChat.Infrastructure.Database.Repositories;
using MealMind.Modules.AiChat.Infrastructure.Database.Seeders;
using MealMind.Modules.AiChat.Infrastructure.Services;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Infrastructure.Postgres;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenAI;

namespace MealMind.Modules.AiChat.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddPostgres<AiChatDbContext>()
            .AddScoped<IAiChatDbContext, AiChatDbContext>()
            .AddUnitOfWork<IUnitOfWork, UnitOfWork>()
            .AddScoped<IEmbeddingService, EmbeddingService>()
            .AddScoped<IChunkingService, ChunkingService>()
            .AddScoped<IConversationRepository, ConversationRepository>()
            .AddScoped<IDocumentRepository, DocumentRepository>()
            .AddScoped<IAiChatUserRepository, AiChatUserRepository>()
            .AddScoped<IAiChatService, AiChatService>()
            .AddTransient<IModuleSeeder, AiChatModuleSeeder>();

        return services;
    }
}