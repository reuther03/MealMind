using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Domain.Conversation;
using MealMind.Shared.Infrastructure.Postgres;

namespace MealMind.Modules.AiChat.Infrastructure.Database.Repositories;

internal class ConversationRepository : Repository<Conversation, AiChatDbContext>, IConversationRepository
{
    public ConversationRepository(AiChatDbContext dbContext) : base(dbContext)
    {
    }
}