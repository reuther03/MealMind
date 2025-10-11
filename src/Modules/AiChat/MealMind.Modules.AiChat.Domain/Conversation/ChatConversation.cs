using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.AiChat.Domain.Conversation;

public class ChatConversation : Entity<Guid>
{
    public UserId UserId { get; private set; }
    public string? Title { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUsedAt { get; private set; }

    private ChatConversation()
    {
    }

    private ChatConversation(Guid id, UserId userId, string? title, DateTime createdAt, DateTime lastUsedAt) : base(id)
    {
        UserId = userId;
        Title = title;
        CreatedAt = createdAt;
        LastUsedAt = lastUsedAt;
    }

    public static ChatConversation Create(UserId userId, string? title)
        => new(Guid.NewGuid(), userId, title, DateTime.UtcNow, DateTime.UtcNow);
}