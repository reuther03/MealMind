using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Modules.AiChat.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Modules.AiChat.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        var options = new AiChatOptions();
        configuration.GetSection(AiChatOptions.SectionName).Bind(options);

        services.AddOllamaChatClient(options.ChatModel, new Uri(options.Uri));
        services.AddOllamaEmbeddingGenerator(options.EmbedModel, new Uri(options.Uri));

        services.AddScoped<IResponseManager, ResponseManager>();

        return services;
    }
}