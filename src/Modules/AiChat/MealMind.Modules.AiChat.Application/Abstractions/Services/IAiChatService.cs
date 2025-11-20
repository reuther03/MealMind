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

    Task<string> GenerateTextToImagePromptAsync(
        string userPrompt, // will be structured or plain text// structured like "product, weight, way of cooking etc."
        IFormFile imageFile, // the image, now is like this but mayby store in url
        CancellationToken cancellationToken = default);
}