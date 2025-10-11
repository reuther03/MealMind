using MealMind.Modules.AiChat.Domain.Conversation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealMind.Modules.AiChat.Infrastructure.Database.Configurations;

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("ChatMessages");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ConversationId)
            .IsRequired();

        builder.Property(x => x.Role)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.Content)
            .IsRequired();

        builder.Property(x => x.ReplyToMessageId)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        // Composite index for efficient conversation history queries
        builder.HasIndex(x => new { x.ConversationId, x.CreatedAt });
    }
}