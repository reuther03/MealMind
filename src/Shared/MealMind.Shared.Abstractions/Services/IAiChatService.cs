namespace MealMind.Shared.Abstractions.Services;

public interface IAiChatService
{
    Task<string> GetResponseAsync(string prompt, CancellationToken cancellationToken = default);
}