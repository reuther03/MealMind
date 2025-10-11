using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Infrastructure.Database;
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
        var options = new AiChatOptions();
        configuration.GetSection(AiChatOptions.SectionName).Bind(options);

        services.AddOllamaChatClient(options.ChatModel, new Uri(options.Uri));
        services.AddOllamaEmbeddingGenerator(options.EmbedModel, new Uri(options.Uri));

        services.AddSingleton<IAiChatService, AiChatService>();

        services.AddPostgres<AiChatDbContext>()
            .AddScoped<IAiChatDbContext, AiChatDbContext>()
            .AddScoped<IAiChatService, AiChatService>();

        return services;
    }
}