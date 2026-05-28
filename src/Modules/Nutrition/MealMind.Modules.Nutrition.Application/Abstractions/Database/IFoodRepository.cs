using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Shared.Abstractions.Kernel.Database;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.Nutrition.Application.Abstractions.Database;

public interface IFoodRepository : IRepository<Food>
{
    Task<Food?> GetByIdAsync(FoodId id, UserId userId, CancellationToken cancellationToken);
    Task<Food?> GetByBarcodeAsync(string barcode, UserId userId, CancellationToken cancellationToken);
}