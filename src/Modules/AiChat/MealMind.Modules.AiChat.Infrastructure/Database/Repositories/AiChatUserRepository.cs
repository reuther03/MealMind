using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Domain.AiChatUser;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using MealMind.Shared.Infrastructure.Postgres;

namespace MealMind.Modules.AiChat.Infrastructure.Database.Repositories;

internal class AiChatUserRepository : Repository<AiChatUser, AiChatDbContext>, IAiChatUserRepository
{
    private readonly AiChatDbContext _dbContext;

    public AiChatUserRepository(AiChatDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AiChatUser?> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken)
        => await _dbContext.AiChatUsers.FindAsync([userId], cancellationToken);
}