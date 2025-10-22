using System.Text.RegularExpressions;
using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Modules.AiChat.Application.Dtos;

namespace MealMind.Modules.AiChat.Application.Services;

internal sealed class ResponseManager : IResponseManager
{
    public Task<StructuredResponse> GenerateStructuredResponseAsync(string userPrompt, string documentsText = "", CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}