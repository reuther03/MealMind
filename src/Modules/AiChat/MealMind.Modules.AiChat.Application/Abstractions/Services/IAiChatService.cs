using MealMind.Modules.AiChat.Application.Dtos;
using MealMind.Shared.Contracts.Dto.AiChat;
using MealMind.Shared.Contracts.Dto.Nutrition;
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

    Task<AnalyzedImageStructuredResponse> AnalyzeImageWithPromptAsync(
        string? userPrompt,
        List<UserProvidedFoodProductsPayload> detectedFoods,
        IFormFile imageFile,
        CancellationToken cancellationToken = default);

    Task<FoodDto> CreateFoodFromPromptAsync(
        string userPrompt,
        CancellationToken cancellationToken = default);
}