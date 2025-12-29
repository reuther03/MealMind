namespace MealMind.Shared.Contracts.Dto.Nutrition;

public class SearchFoodDto
{
    public Guid? Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Barcode { get; init; }
    public string? Brand { get; init; }
    public string FoodSource { get; init; } = null!;
}