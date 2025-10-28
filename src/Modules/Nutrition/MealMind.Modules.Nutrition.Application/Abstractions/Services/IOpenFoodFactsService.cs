using MealMind.Modules.Nutrition.Application.Dtos;

namespace MealMind.Modules.Nutrition.Application.Abstractions.Services;

public interface IOpenFoodFactsService
{
    Task<List<FoodDto>> SearchFoodByNameAsync(string name, int pageSize, int page, CancellationToken cancellationToken = default);
    Task<FoodDto> GetFoodByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);

    Task<List<FoodDto>> SearchFoodByNameWithoutDuplicatesAsync(string name, int pageSize, int page, List<FoodDto> existingFoods,
        CancellationToken cancellationToken = default);
}