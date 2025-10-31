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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Modules.AiChat.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        var options = new OpenRouterModelOptions();
        configuration.GetSection(OpenRouterModelOptions.SectionName).Bind(options);

        services.AddPostgres<AiChatDbContext>()
            .AddScoped<IAiChatDbContext, AiChatDbContext>()
            .AddUnitOfWork<IUnitOfWork, UnitOfWork>()
            .AddScoped<IEmbeddingService, EmbeddingService>()
            .AddScoped<IChunkingService, ChunkingService>()
            .AddScoped<IConversationRepository, ConversationRepository>()
            .AddScoped<IDocumentRepository, DocumentRepository>()
            .AddScoped<IAiChatUserRepository, AiChatUserRepository>()
            .AddTransient<IModuleSeeder, AiChatModuleSeeder>()
            .AddHttpClient<IAiChatService, AiChatService>(opt =>
            {
                opt.BaseAddress = new Uri(options.BaseUrl);
                opt.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.ApiKey}");
            });

        return services;
    }
}