using MealMind.Modules.AiChat.Application.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.SemanticKernel;

namespace MealMind.Modules.AiChat.Application.Abstractions.Services;

public interface IAiChatService
{
    Task<StructuredResponse> GenerateStructuredResponseAsync(
        string userPrompt,
        string documentsText,
        List<ChatMessageContent> chatMessages,
        int responseTokensLimit,
        CancellationToken cancellationToken = default);

    Task<AnalyzedImageStructuredResponse> GenerateTextToImagePromptAsync(
        string? userPrompt,
        IFormFile imageFile,
        CancellationToken cancellationToken = default);
}