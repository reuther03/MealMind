using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Domain.Conversation;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using MealMind.Shared.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.AiChat.Infrastructure.Database.Repositories;

internal class ConversationRepository : Repository<Conversation, AiChatDbContext>, IConversationRepository
{
    private readonly AiChatDbContext _dbContext;

    public ConversationRepository(AiChatDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Conversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbContext.ChatConversations
            .Include(x => x.ChatMessages)
            .FirstOrDefaultAsync(x => x.Id == ConversationId.From(id), cancellationToken);

    public async Task<int> GetUserDailyConversationPromptsCountAsync(Guid aiChatUserId, CancellationToken cancellationToken = default)
        => await _dbContext.ChatConversations
            .Include(x => x.ChatMessages)
            .Where(x => x.UserId == UserId.From(aiChatUserId))
            .SelectMany(x => x.ChatMessages
                .Where(z => z.Role == AiChatRole.User && z.CreatedAt.Date == DateTime.UtcNow.Date))
            .CountAsync(cancellationToken);
}