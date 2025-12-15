using MealMind.Modules.AiChat.Application.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Modules.AiChat.Application;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication(IConfiguration configuration)
        {
            var options = new OllamaAiChatOptions();
            configuration.GetSection(OllamaAiChatOptions.SectionName).Bind(options);

            // services.AddOllamaChatClient(options.ChatModel, new Uri(options.Uri));
            services.AddOllamaEmbeddingGenerator(options.EmbedModel, new Uri(options.Uri));

            // services.AddScoped<IResponseManager, ResponseManager>();

            return services;
        }
    }
}