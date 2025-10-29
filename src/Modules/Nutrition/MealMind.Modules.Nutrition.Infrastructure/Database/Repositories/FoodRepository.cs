using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Domain.Food;
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

    public async Task<Food?> GetByIdAsync(FoodId id, CancellationToken cancellationToken)
        => await _context.Foods.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddIfNotExistsAsync(Food food, CancellationToken cancellationToken)
    {
        if (await _context.Foods.AnyAsync(x => x.Barcode == food.Barcode, cancellationToken) ||
            await _context.Foods.AnyAsync(x => x.Id == food.Id, cancellationToken))
            return;

        await _context.Foods.AddAsync(food, cancellationToken);
    }
}