namespace MealMind.Shared.Abstractions.Kernel.Payloads;

public record NutritionTargetPayload(
    decimal Calories,
    NutritionInGramsPayload? NutritionInGramsPayload,
    NutritionInPercentPayload? NutritionInPercentPayload,
    decimal WaterIntake,
    List<DayOfWeek>? ActiveDays);