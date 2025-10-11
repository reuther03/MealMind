using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.AiChat.Domain.Conversation;

public record ConversationId : EntityId
{
    private ConversationId(Guid value) : base(value)
    {
    }

    public static ConversationId New() => new(Guid.NewGuid());
    public static ConversationId From(Guid value) => new(value);
    public static ConversationId From(string value) => new(Guid.Parse(value));

    public static implicit operator Guid(ConversationId conversationId) => conversationId.Value;
    public static implicit operator ConversationId(Guid userId) => new(userId);

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}