using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.Nutrition.Domain.Tracking;

public record MealId : EntityId
{
    private MealId(Guid value) : base(value)
    {
    }

    public static MealId New() => new(Guid.NewGuid());
    public static MealId From(Guid value) => new(value);
    public static MealId From(string value) => new(Guid.Parse(value));

    public static implicit operator Guid(MealId mealId) => mealId.Value;
    public static implicit operator MealId(Guid mealId) => new(mealId);

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}