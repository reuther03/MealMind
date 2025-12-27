namespace MealMind.Shared.Contracts.Dto.Nutrition;

public class FoodDto
{
    public Guid? Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Barcode { get; init; }
    public string? Brand { get; init; }
    public string? ImageUrl { get; init; }
    public NutrimentsPer100GDto NutritionPer100G { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public string FoodSource { get; init; } = null!;
}