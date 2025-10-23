using MealMind.Modules.AiChat.Domain.Rag;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealMind.Modules.AiChat.Infrastructure.Database.Configurations;

public class RagDocumentConfiguration : IEntityTypeConfiguration<RagDocument>
{
    public void Configure(EntityTypeBuilder<RagDocument> builder)
    {
        builder.ToTable("RagDocuments");
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
            .HasColumnType("vector(1024)");

        builder.Property(x => x.ChunkIndex)
            .IsRequired();

        builder.Property(x => x.DocumentGroupId)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.AttachedAt)
            .IsRequired();

        // Index for semantic search
        builder.HasIndex(x => x.Embedding)
            .HasMethod("hnsw")
            .HasOperators("vector_cosine_ops");

        // Index for grouping chunks
        builder.HasIndex(x => new { x.DocumentGroupId, x.ChunkIndex });
    }
}