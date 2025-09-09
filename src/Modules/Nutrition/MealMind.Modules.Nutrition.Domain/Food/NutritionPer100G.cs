using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.Nutrition.Domain.Food;

public record NutritionPer100G : ValueObject
{
    public decimal Calories { get; private set; }
    public decimal Protein { get; private set; }
    public decimal Carbohydrates { get; private set; }
    public decimal? Sugar { get; private set; }

    public decimal Fat { get; private set; }

    public decimal? SaturatedFat { get; private set; }

    public decimal? Fiber { get; private set; }
    public decimal? Sodium { get; private set; }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Calories;
        yield return Protein;
        yield return Carbohydrates;
        yield return Sugar;
        yield return Fat;
        yield return SaturatedFat;
        yield return Fiber;
        yield return Sodium;
    }
}