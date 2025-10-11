namespace MealMind.Shared.Abstractions.Services;

public interface IAiChatService
{
    Task<string> GetResponseAsync(string prompt, Guid conversationId, CancellationToken cancellationToken = default);
    // Task<string> StartConversationAsync(string systemPrompt, CancellationToken cancellationToken = default);
}