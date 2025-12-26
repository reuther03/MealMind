using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using MealMind.Shared.Contracts.Dto.AiChat;

namespace MealMind.Modules.AiChat.Domain.Conversation;

public class Conversation : AggregateRoot<ConversationId>
{
    private readonly IList<AiChatMessage> _chatMessages = [];
    public UserId UserId { get; private set; }
    public string Title { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUsedAt { get; private set; }
    public IReadOnlyList<AiChatMessage> ChatMessages => _chatMessages.AsReadOnly();

    private Conversation()
    {
    }

    private Conversation(ConversationId id, UserId userId) : base(id)
    {
        UserId = userId;
        Title = "";
        CreatedAt = DateTime.UtcNow;
        LastUsedAt = CreatedAt;
    }

    public static Conversation Create(UserId userId)
        => new(ConversationId.New(), userId);

    public void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or whitespace.", nameof(title));

        Title = title;
    }

    public void SetLastUsedAt(DateTime lastUsedAt)
    {
        if (lastUsedAt < CreatedAt)
            throw new ArgumentException("Last used date cannot be earlier than creation date.", nameof(lastUsedAt));

        LastUsedAt = lastUsedAt;
    }

    public void AddMessage(AiChatMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        if (message.ConversationId != Id)
            throw new InvalidOperationException("Message does not belong to this conversation.");

        _chatMessages.Add(message);
        LastUsedAt = DateTime.UtcNow;
    }

    public AiChatMessage GetRecentMessage()
    {
        return !_chatMessages.Any()
            ? throw new InvalidOperationException("No messages in the conversation.")
            : _chatMessages.Where(x => x.Role == AiChatRole.Assistant).OrderByDescending(x => x.CreatedAt).First();
    }
}