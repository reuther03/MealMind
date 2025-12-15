namespace MealMind.Modules.AiChat.Application.Dtos;

public record UserProvidedFoodProductsPayload
{
    public string FoodName { get; init; } = string.Empty;
    public decimal QuantityInGrams { get; init; }
    public decimal MinCalories { get; init; }
    public decimal MaxCalories { get; init; }
    public decimal MinProteins { get; init; }
    public decimal MaxProteins { get; init; }
    public decimal MinFats { get; init; }
    public decimal MaxFats { get; init; }
    public decimal MinCarbohydrates { get; init; }
    public decimal MaxCarbohydrates { get; init; }
}