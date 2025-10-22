using MealMind.Modules.AiChat.Application.Dtos;

namespace MealMind.Modules.AiChat.Application.Abstractions.Services;

public interface IResponseManager
{
    Task<StructuredResponse> GenerateStructuredResponseAsync(string userPrompt, string documentsText = "", CancellationToken cancellationToken = default);
}