using MealMind.Services.AiChat.Services;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Services.AiChat;

public static class Extensions
{
    public static IServiceCollection AddAiChat(this IServiceCollection services)
    {
        var options = services.GetOptions<AiChatOptions>(AiChatOptions.SectionName);

        services.AddOllamaChatClient(options.ChatModel, new Uri(options.Uri));
        services.AddOllamaEmbeddingGenerator(options.EmbedModel, new Uri(options.Uri));

        services.AddSingleton<IAiChatService, AiChatService>();

        return services;
    }
}