using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Shared.Abstractions.Kernel.Database;

namespace MealMind.Modules.Nutrition.Application.Abstractions.Database;

public interface IFoodRepository : IRepository<Food>
{
    Task<Food?> GetByIdAsync(FoodId id, CancellationToken cancellationToken);
    Task<Food?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken);
}