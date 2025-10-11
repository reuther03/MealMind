using MealMind.Modules.AiChat.Domain.Conversation;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.AiChat.Application.Abstractions.Database;

public interface IAiChatDbContext
{
    DbSet<Conversation> ChatConversations { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}