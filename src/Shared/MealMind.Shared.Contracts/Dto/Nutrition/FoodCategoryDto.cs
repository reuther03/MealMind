namespace MealMind.Shared.Contracts.Dto.Nutrition;

public sealed class FoodCategoryDto
{
    public Guid Id { get; init; }
    public string Category { get; init; } = null!;
}
