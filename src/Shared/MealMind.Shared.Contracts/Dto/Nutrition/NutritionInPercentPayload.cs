namespace MealMind.Shared.Contracts.Dto.Nutrition;

public record NutritionInPercentPayload(
    decimal ProteinInPercent,
    decimal CarbohydratesInPercent,
    decimal FatsInPercent);