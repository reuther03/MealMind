using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Domain.Conversation;
using MealMind.Modules.AiChat.Domain.Rag;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.AiChat.Infrastructure.Database;

internal class AiChatDbContext : DbContext, IAiChatDbContext
{
    public DbSet<Conversation> ChatConversations => Set<Conversation>();
    public DbSet<RagDocument> RagDocuments => Set<RagDocument>();
    public DbSet<ConversationDocument> ConversationDocuments => Set<ConversationDocument>();

    public AiChatDbContext(DbContextOptions<AiChatDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        modelBuilder.HasDefaultSchema("aichat");
    }
}