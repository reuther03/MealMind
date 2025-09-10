using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

public record UserId : EntityId
{
    private UserId(Guid value) : base(value)
    {
    }

    public static UserId New() => new(Guid.NewGuid());
    public static UserId From(Guid value) => new(value);
    public static UserId From(string value) => new(Guid.Parse(value));

    public static implicit operator Guid(UserId userId) => userId.Value;
    public static implicit operator UserId(Guid userId) => new(userId);

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}