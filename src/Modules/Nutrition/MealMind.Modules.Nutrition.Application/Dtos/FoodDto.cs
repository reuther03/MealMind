using MealMind.Modules.Nutrition.Domain.Food;

namespace MealMind.Modules.Nutrition.Application.Dtos;

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

    public static FoodDto AsDto(Food food)
    {
        return new FoodDto
        {
            Id = food.Id,
            Name = food.Name.Value,
            Barcode = food.Barcode,
            Brand = food.Brand,
            ImageUrl = food.ImageUrl,
            NutritionPer100G = NutrimentsPer100GDto.AsDto(food.NutritionPer100G),
            CreatedAt = food.CreatedAt,
            FoodSource = food.Source.ToString()
        };
    }

    public static Food ToEntity(FoodDto dto)
    {
        return Food.Create(
            dto.Name,
            dto.NutritionPer100G.ToEntity(dto.NutritionPer100G),
            Source.ExternalApi,
            dto.Barcode,
            dto.Brand,
            dto.ImageUrl
        );
    }
}