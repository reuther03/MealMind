using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.AiChat.Domain.Conversation;

public class Conversation : AggregateRoot<ConversationId>
{
    private readonly IList<ChatMessage> _chatMessages = [];
    public UserId UserId { get; private set; }
    public string? Title { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUsedAt { get; private set; }
    public IReadOnlyList<ChatMessage> ChatMessages => _chatMessages.ToList().AsReadOnly();

    private Conversation()
    {
    }

    private Conversation(ConversationId id, UserId userId, string? title, DateTime createdAt, DateTime lastUsedAt) : base(id)
    {
        UserId = userId;
        Title = title;
        CreatedAt = createdAt;
        LastUsedAt = lastUsedAt;
    }

    public static Conversation Create(UserId userId, string? title, DateTime createdAt, DateTime lastUsedAt)
        => new(ConversationId.New(), userId, title, createdAt, lastUsedAt);

    public void AddMessage(ChatMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        if (message.ConversationId != Id)
            throw new InvalidOperationException("Message does not belong to this conversation.");

        _chatMessages.Add(message);
        LastUsedAt = DateTime.UtcNow;
    }
}