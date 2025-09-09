using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.Nutrition.Domain.Food;

public record NutritionPer100G : ValueObject
{
    public decimal Calories { get; }
    public decimal Protein { get; }
    public decimal Fat { get; }

    public decimal? SaturatedFat { get; }
    public decimal Carbohydrates { get; }
    public decimal? Sugar { get; }


    public decimal? Fiber { get; }
    public decimal? Sodium { get; }
    public decimal? Cholesterol { get; }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Calories;
        yield return Protein;
        yield return Carbohydrates;
        yield return Sugar ?? 0;
        yield return Fat;
        yield return SaturatedFat ?? 0;
        yield return Fiber ?? 0;
        yield return Sodium ?? 0;
        yield return Cholesterol ?? 0;
    }
}