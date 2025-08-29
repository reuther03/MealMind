using MealMind.Modules.Identity.Application.Abstractions;
using MealMind.Modules.Identity.Infrastructure.Database;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Infrastructure.Postgres;

namespace MealMind.Modules.Identity.Infrastructure;

internal class UnitOfWork : BaseUnitOfWork<IdentityDbContext>, IUnitOfWork
{
    public UnitOfWork(IdentityDbContext context, IPublisher publisher) : base(context, publisher)
    {
    }
}