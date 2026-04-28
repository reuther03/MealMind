namespace MealMind.Shared.Contracts.Dto.Nutrition;

public sealed class FoodDietaryTagDto
{
    public Guid Id { get; init; }
    public string DietaryTag { get; init; } = null!;
}
