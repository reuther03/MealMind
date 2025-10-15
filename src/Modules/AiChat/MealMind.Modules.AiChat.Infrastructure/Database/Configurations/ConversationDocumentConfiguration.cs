using MealMind.Modules.AiChat.Domain.Conversation;
using MealMind.Modules.AiChat.Domain.Rag;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealMind.Modules.AiChat.Infrastructure.Database.Configurations;

public class ConversationDocumentConfiguration : IEntityTypeConfiguration<ConversationDocument>
{
    public void Configure(EntityTypeBuilder<ConversationDocument> builder)
    {
        builder.ToTable("ConversationDocuments");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Content)
            .HasMaxLength(5000)
            .IsRequired();

        builder.Property(x => x.Embedding)
            .HasColumnType("vector(768)");

        builder.Property(x => x.ChunkIndex)
            .IsRequired();

        builder.Property(x => x.DocumentGroupId)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.AttachedAt)
            .IsRequired();

        builder.Property(x => x.ConversationId)
            .HasConversion(x => x.Value, x => ConversationId.From(x))
            .ValueGeneratedNever()
            .IsRequired();

        // Index for semantic search within a conversation
        builder.HasIndex(x => x.Embedding)
            .HasMethod("hnsw")
            .HasOperators("vector_cosine_ops");

        // Index for getting all documents for a conversation
        builder.HasIndex(x => new { x.ConversationId, x.DocumentGroupId, x.ChunkIndex });
    }
}