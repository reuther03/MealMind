using System.Net.Http.Json;
using MealMind.Modules.AiChat.Application;
using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Modules.AiChat.Application.Dtos;
using MealMind.Modules.AiChat.Application.Options;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace MealMind.Modules.AiChat.Infrastructure.Services;

public class AiChatService : IAiChatService
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<OpenRouterModelOptions> _openRouterModelOptions;

    public AiChatService(HttpClient httpClient, IOptions<OpenRouterModelOptions> openRouterModelOptions)
    {
        _httpClient = httpClient;
        _openRouterModelOptions = openRouterModelOptions;
    }

    public async Task<StructuredResponse> GenerateStructuredResponseAsync(string userPrompt, string documentsText, List<string> documentTitles,
        List<ChatMessage> chatMessages, int responseTokensLimit,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/chat/completions", new
        {
            model = _openRouterModelOptions.Value.BaseModel,
        });

        throw new NotImplementedException();
    }
}