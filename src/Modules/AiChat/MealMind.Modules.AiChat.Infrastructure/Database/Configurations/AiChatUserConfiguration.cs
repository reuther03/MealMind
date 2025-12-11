using MealMind.Modules.AiChat.Domain.AiChatUser;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealMind.Modules.AiChat.Infrastructure.Database.Configurations;

public class AiChatUserConfiguration : IEntityTypeConfiguration<AiChatUser>
{
    public void Configure(EntityTypeBuilder<AiChatUser> builder)
    {
        builder.ToTable("AiChatUsers");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasConversion(id => id.Value, value => UserId.From(value))
            .ValueGeneratedNever();

        builder.Property(u => u.Tier)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(u => u.ActiveConversations)
            .IsRequired();

        builder.Property(u => u.ConversationsLimit)
            .IsRequired();

        builder.Property(u => u.ConversationsMessagesHistoryDaysLimit)
            .IsRequired();

        builder.Property(u => u.DocumentsLimit)
            .IsRequired();

        builder.Property(u => u.PromptTokensLimit)
            .IsRequired();

        builder.Property(u => u.ResponseTokensLimit)
            .IsRequired();

        builder.Property(u => u.DailyPromptsLimit)
            .IsRequired();

        builder.Property(u => u.CanExportData)
            .IsRequired();

        builder.Property(u => u.CanUseAdvancedPrompts)
            .IsRequired();

        builder.Property(u => u.DailyImageAnalysisLimit)
            .IsRequired();

        builder.Property(u => u.ImageAnalysisCorrectionPromptLimit)
            .IsRequired();

        builder.Property(u => u.StartDate)
            .IsRequired();

        builder.Property(u => u.EndDate);
    }
}