using MealMind.Modules.Nutrition.Application.Dtos;
using MealMind.Modules.Nutrition.Domain.Food;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MealMind.Modules.Nutrition.Application.Abstractions.Services;

public interface IOpenFoodFactsService
{
    Task<List<FoodDto>> SearchFoodByNameAsync(string name, int pageSize, int page, CancellationToken cancellationToken = default);
}