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

    public NutritionPer100G(decimal calories, decimal protein, decimal fat, decimal carbohydrates, decimal? sugar = null,
        decimal? saturatedFat = null, decimal? fiber = null, decimal? sodium = null, decimal? cholesterol = null)
    {
        if (calories < 0) throw new ArgumentException("Calories cannot be negative", nameof(calories));
        if (protein < 0) throw new ArgumentException("Protein cannot be negative", nameof(protein));
        if (fat < 0) throw new ArgumentException("Fat cannot be negative", nameof(fat));
        if (carbohydrates < 0) throw new ArgumentException("Carbohydrates cannot be negative", nameof(carbohydrates));
        if (sugar < 0) throw new ArgumentException("Sugar cannot be negative", nameof(sugar));
        if (saturatedFat < 0) throw new ArgumentException("Saturated fat cannot be negative", nameof(saturatedFat));
        if (fiber < 0) throw new ArgumentException("Fiber cannot be negative", nameof(fiber));
        if (sodium < 0) throw new ArgumentException("Sodium cannot be negative", nameof(sodium));
        if (cholesterol < 0) throw new ArgumentException("Cholesterol cannot be negative", nameof(cholesterol));

        var calculatedCalories = protein * 4 + carbohydrates * 4 + fat * 9;
        if (calculatedCalories > calories * 1.1m)
            throw new ArgumentException("Macro calories exceed total calories by more than 10%");

        Calories = calories;
        Protein = protein;
        Fat = fat;
        Carbohydrates = carbohydrates;
        Sugar = sugar;
        SaturatedFat = saturatedFat;
        Fiber = fiber;
        Sodium = sodium;
        Cholesterol = cholesterol;
    }

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