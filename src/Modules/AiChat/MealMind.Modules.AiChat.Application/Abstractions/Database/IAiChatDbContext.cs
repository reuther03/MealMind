using MealMind.Modules.AiChat.Domain.Conversation;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.AiChat.Application.Abstractions.Database;

public interface IAiChatDbContext
{
    DbSet<ChatConversation> ChatConversations { get; }
    DbSet<ChatMessage> ChatMessages { get; }
}