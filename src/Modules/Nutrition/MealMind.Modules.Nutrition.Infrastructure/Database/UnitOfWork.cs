using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Infrastructure.Postgres;

namespace MealMind.Modules.Nutrition.Infrastructure.Database;

internal class UnitOfWork : BaseUnitOfWork<NutritionDbContext>, IUnitOfWork
{
    public UnitOfWork(NutritionDbContext context, IPublisher publisher) : base(context, publisher)
    {
    }
}