using MealMind.Modules.AiChat.Domain.AiChatUser;
using MealMind.Modules.AiChat.Domain.Conversation;
using MealMind.Modules.AiChat.Domain.ImageAnalyze;
using MealMind.Modules.AiChat.Domain.Rag;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.AiChat.Application.Abstractions.Database;

public interface IAiChatDbContext
{
    DbSet<Conversation> ChatConversations { get; }
    DbSet<RagDocument> RagDocuments { get; }
    DbSet<ConversationDocument> ConversationDocuments { get; }
    DbSet<AiChatUser> AiChatUsers { get; }
    DbSet<ImageAnalyzeSession> FoodImageAnalyzeSessions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}