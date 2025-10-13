using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Domain.Conversation;
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
}