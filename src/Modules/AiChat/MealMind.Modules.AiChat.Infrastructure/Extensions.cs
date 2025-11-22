using MealMind.Modules.AiChat.Application.Abstractions;
using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Modules.AiChat.Application.Options;
using MealMind.Modules.AiChat.Infrastructure.Database;
using MealMind.Modules.AiChat.Infrastructure.Database.Repositories;
using MealMind.Modules.AiChat.Infrastructure.Database.Seeders;
using MealMind.Modules.AiChat.Infrastructure.Services;
using MealMind.Modules.AiChat.Infrastructure.Services.AIChatService;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Infrastructure.Postgres;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Connectors.OpenAI;


namespace MealMind.Modules.AiChat.Infrastructure;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure()
        {
            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            // var options = new OpenRouterModelOptions();
            // configuration.GetSection(OpenRouterModelOptions.SectionName).Bind(options);

            var options = new GeminiOptions();
            configuration.GetSection(GeminiOptions.SectionName).Bind(options);


            services.AddPostgres<AiChatDbContext>()
                .AddScoped<IAiChatDbContext, AiChatDbContext>()
                .AddUnitOfWork<IUnitOfWork, UnitOfWork>()
                .AddScoped<IEmbeddingService, EmbeddingService>()
                .AddScoped<IChunkingService, ChunkingService>()
                .AddScoped<IAiChatService, AiChatService>()
                .AddScoped<IConversationRepository, ConversationRepository>()
                .AddScoped<IDocumentRepository, DocumentRepository>()
                .AddScoped<IAiChatUserRepository, AiChatUserRepository>()
                .AddScoped<IImageAnalyzeRepository, ImageAnalyzeRepository>()
                .AddTransient<IModuleSeeder, AiChatModuleSeeder>();


            // Register Chat Completion Service with OpenRouter
            // services.AddSingleton<IChatCompletionService>(sp => new OpenAIChatCompletionService(
            //     modelId: options.BaseModel,
            //     endpoint: new Uri(options.BaseUrl),
            //     apiKey: options.ApiKey));

            // Register Chat Completion Service with Google Gemini
            services.AddSingleton<IChatCompletionService>(sp => new GoogleAIGeminiChatCompletionService(
                modelId: options.Model,
                apiKey: options.ApiKey
            ));

            return services;
        }
    }
}