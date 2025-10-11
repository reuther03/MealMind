namespace MealMind.Shared.Abstractions.Services;

public interface IAiChatService
{
    Task<string> GetResponseAsync(string prompt, Guid conversationId, CancellationToken cancellationToken = default);
}