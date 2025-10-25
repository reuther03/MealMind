using MealMind.Modules.AiChat.Application.Dtos;
using Microsoft.Extensions.AI;

namespace MealMind.Modules.AiChat.Application.Abstractions.Services;

public interface IResponseManager
{
    Task<StructuredResponse> GenerateStructuredResponseAsync(
        string userPrompt,
        string documentsText,
        List<string> documentTitles,
        List<ChatMessage> chatMessages,
        int responseTokensLimit,
        CancellationToken cancellationToken = default);
}