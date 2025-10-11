using MealMind.Modules.AiChat.Application.Abstractions;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Infrastructure.Postgres;

namespace MealMind.Modules.AiChat.Infrastructure.Database;

internal class UnitOfWork : BaseUnitOfWork<AiChatDbContext>, IUnitOfWork
{
    public UnitOfWork(AiChatDbContext context, IPublisher publisher) : base(context, publisher)
    {
    }
}