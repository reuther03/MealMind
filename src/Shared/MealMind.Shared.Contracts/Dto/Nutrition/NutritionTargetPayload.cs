namespace MealMind.Shared.Contracts.Dto.Nutrition;

public record NutritionTargetPayload(
    decimal Calories,
    NutritionInGramsPayload? NutritionInGramsPayload,
    NutritionInPercentPayload? NutritionInPercentPayload,
    decimal WaterIntake,
    List<DayOfWeek>? ActiveDays);