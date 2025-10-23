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
        CancellationToken cancellationToken = default);

    Task<string> AttemptJsonCorrectionAsync(string originalQuestion, string malformedJson, string documentsText,
        CancellationToken cancellationToken = default);
}