using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using MealMind.Shared.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Repositories;

internal class FoodRepository : Repository<Food, NutritionDbContext>, IFoodRepository
{
    private readonly NutritionDbContext _context;

    public FoodRepository(NutritionDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<Food?> GetByIdAsync(FoodId id, UserId userId, CancellationToken cancellationToken)
        => await _context.Foods
            .Where(x => !x.IsPrivate || x.CreatedBy == userId)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<Food?> GetByBarcodeAsync(string barcode, UserId userId, CancellationToken cancellationToken)
        => await _context.Foods
            .Where(x => !x.IsPrivate || x.CreatedBy == userId)
            .FirstOrDefaultAsync(x => x.Barcode == barcode, cancellationToken);
}