using MealMind.Modules.AiChat.Domain.AiChatUser;
using MealMind.Shared.Abstractions.Kernel.Database;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.AiChat.Application.Abstractions.Database;

public interface IAiChatUserRepository : IRepository<AiChatUser>
{
    Task<AiChatUser?> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken);
}