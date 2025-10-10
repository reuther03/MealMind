using MealMind.Shared.Abstractions.Services;

namespace MealMind.Services.AiChat.Services;

public class AiChatService : IAiChatService
{
    public Task<string> GenerateHashtags(string postContent, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}