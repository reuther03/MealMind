using MealMind.Modules.AiChat.Domain.Rag;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace MealMind.Modules.AiChat.Infrastructure.Database.Configurations;

public class RagDocumentConfiguration : IEntityTypeConfiguration<RagDocument>
{
    public void Configure(EntityTypeBuilder<RagDocument> builder)
    {
        builder.ToTable("RagDocument");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Content)
            .IsRequired();

        builder.Property(x => x.Embedding)
            .HasColumnType("vector(768)") // Assuming 768 dimensions for the embedding
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}