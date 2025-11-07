using System.Diagnostics.CodeAnalysis;
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
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;


namespace MealMind.Modules.AiChat.Infrastructure;

public static class Extensions
{
    [Experimental("SKEXP0010")]
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
            .AddTransient<IModuleSeeder, AiChatModuleSeeder>();

        services.AddSingleton<IChatCompletionService>(sp => new OpenAIChatCompletionService(
            options.VisionModel, new Uri(options.BaseUrl), options.ApiKey));

        // services.AddSingleton<IChatCompletionService>(sp =>
        // {
        //     var options = sp.GetRequiredService<IOptions<OpenRouterModelOptions>>().Value;
        //
        //     return new OpenAIChatCompletionService(
        //         modelId: options.BaseModel,
        //         endpoint: new Uri(options.BaseUrl),
        //         apiKey: options.ApiKey
        //     );
        // });

        // services.AddSingleton<Kernel>(sp => Kernel.CreateBuilder()
        //     .AddOpenAIChatCompletion(options.VisionModel, new Uri(options.BaseUrl), options.ApiKey)
        //     .Build());


        return services;
    }
}