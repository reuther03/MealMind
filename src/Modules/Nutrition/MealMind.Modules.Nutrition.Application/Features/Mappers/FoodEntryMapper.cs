using System.Linq.Expressions;
using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Shared.Contracts.Dto.Nutrition;

namespace MealMind.Modules.Nutrition.Application.Features.Mappers;

public static class FoodEntryMapper
{
    public static Expression<Func<FoodEntry, FoodEntryDto>> Projection { get; }
        = entity => new FoodEntryDto
        {
            FoodId = entity.FoodId != null ? entity.FoodId.Value : null,
            FoodName = entity.FoodName.Value,
            FoodBrand = entity.FoodBrand != null ? entity.FoodBrand.Value : null,
            QuantityInGrams = entity.QuantityInGrams,
            TotalCalories = entity.TotalCalories,
            TotalProteins = entity.TotalProteins,
            TotalCarbohydrates = entity.TotalCarbohydrates,
            TotalSugars = entity.TotalSugars,
            TotalFats = entity.TotalFats,
            TotalSaturatedFats = entity.TotalSaturatedFats,
            TotalFiber = entity.TotalFiber,
            TotalSodium = entity.TotalSodium,
            TotalSalt = entity.TotalSalt,
            TotalCholesterol = entity.TotalCholesterol,
            Source = entity.Source.ToString()
        };
}
