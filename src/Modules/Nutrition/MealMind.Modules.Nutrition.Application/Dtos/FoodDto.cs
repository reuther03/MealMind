using MealMind.Modules.Nutrition.Domain.Food;

namespace MealMind.Modules.Nutrition.Application.Dtos;

public class FoodDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Barcode { get; init; }
    public string? Brand { get; init; }
    public string? ImageUrl { get; init; }
    public NutrimentsPer100GDto NutritionPer100G { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public string Source { get; init; } = null!;

    public static FoodDto AsDto(Food food)
    {
        return new FoodDto
        {
            Id = food.Id.Value,
            Name = food.Name.Value,
            Barcode = food.Barcode,
            Brand = food.Brand,
            ImageUrl = food.ImageUrl,
            NutritionPer100G = NutrimentsPer100GDto.AsDto(food.NutritionPer100G),
            CreatedAt = food.CreatedAt,
            Source = food.Source.ToString()
        };
    }
}