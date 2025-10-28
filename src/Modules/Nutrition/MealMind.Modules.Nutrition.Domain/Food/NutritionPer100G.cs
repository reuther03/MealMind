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
    public decimal? Salt { get; }
    public decimal? Cholesterol { get; }

    public NutritionPer100G(decimal calories, decimal protein, decimal fat, decimal carbohydrates, decimal? salt, decimal? sugar = null,
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
        if (salt < 0) throw new ArgumentException("Salt cannot be negative", nameof(salt));
        if (cholesterol < 0) throw new ArgumentException("Cholesterol cannot be negative", nameof(cholesterol));

        // Validate calories are reasonably consistent with macros
        // Allow 30% margin due to:
        // - Fiber (not always subtracted from carbs)
        // - Alcohol content
        // - Rounding errors in external data sources
        // - User-submitted data quality in OpenFoodFacts
        var calculatedCalories = protein * 4 + carbohydrates * 4 + fat * 9;
        var lowerBound = calories * 0.7m;
        var upperBound = calories * 1.3m;

        if (calculatedCalories < lowerBound || calculatedCalories > upperBound)
            throw new ArgumentException(
                $"Calories value ({calories}) is inconsistent with macronutrient values (calculated: {calculatedCalories:F1})",
                nameof(calories));

        Calories = calories;
        Protein = protein;
        Fat = fat;
        Carbohydrates = carbohydrates;
        Salt = salt;
        Sugar = sugar;
        SaturatedFat = saturatedFat;
        Fiber = fiber;
        Sodium = sodium;
        Salt = salt;
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
        yield return Salt ?? 0;
        yield return Cholesterol ?? 0;
    }
}