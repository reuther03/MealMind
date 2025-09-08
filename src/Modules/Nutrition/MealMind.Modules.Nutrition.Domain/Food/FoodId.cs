using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.Nutrition.Domain.Food;

public record FoodId : EntityId
{
    public FoodId(Guid value) : base(value)
    {
    }

    public static FoodId New() => new(Guid.NewGuid());
    public static FoodId From(Guid value) => new(value);
    public static FoodId From(string value) => new(Guid.Parse(value));

    public static implicit operator Guid(FoodId foodId) => foodId.Value;
    public static implicit operator FoodId(Guid foodId) => new(foodId);

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}