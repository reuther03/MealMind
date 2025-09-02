using MealMind.Modules.Identity.Application.Abstractions;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Infrastructure.Postgres;

namespace MealMind.Modules.Identity.Infrastructure.Database;

internal class UnitOfWork : BaseUnitOfWork<IdentityDbContext>, IUnitOfWork
{
    public UnitOfWork(IdentityDbContext context, IPublisher publisher) : base(context, publisher)
    {
    }
}