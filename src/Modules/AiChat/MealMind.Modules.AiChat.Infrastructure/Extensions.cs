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
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;

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

        services.AddSingleton<IChatClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<OpenRouterModelOptions>>().Value;

            var openAiClient = new OpenAIClient(
                credential: new ApiKeyCredential(options.ApiKey),
                options: new OpenAIClientOptions
                {
                    Endpoint = new Uri(options.BaseUrl)
                }
            );

            var chatClient = openAiClient.GetChatClient(options.BaseModel);

            return chatClient.AsIChatClient();
        });

        return services;
    }
}