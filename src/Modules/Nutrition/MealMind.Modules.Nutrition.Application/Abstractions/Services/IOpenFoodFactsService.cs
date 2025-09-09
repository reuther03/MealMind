using MealMind.Modules.Nutrition.Domain.Food;

namespace MealMind.Modules.Nutrition.Application.Abstractions.Services;

public interface IOpenFoodFactsService
{
    Task<List<Food>> SearchFoodByNameAsync(string name, int limit = 20, CancellationToken cancellationToken = default);
}