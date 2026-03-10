using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Infrastructure.Postgres;

namespace MealMind.Modules.Training.Infrastructure.Database;

internal class UnitOfWork : BaseUnitOfWork<TrainingDbContext>, IUnitOfWork
{
    public UnitOfWork(TrainingDbContext context, IPublisher publisher) : base(context, publisher)
    {
    }
}