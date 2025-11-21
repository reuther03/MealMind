using MealMind.Modules.AiChat.Domain.ImageConversation;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealMind.Modules.AiChat.Infrastructure.Database.Configurations;

public class FoodImageAnalyzeConfiguration : IEntityTypeConfiguration<FoodImageAnalyze>
{
    public void Configure(EntityTypeBuilder<FoodImageAnalyze> builder)
    {
        builder.ToTable("FoodImageAnalyze");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasConversion(x => x.Value, x => UserId.From(x))
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Prompt)
            .HasMaxLength(2000);

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(2000);

        builder.Property(x => x.ImageBytes);

        builder.Property(x => x.CaloriesMin)
            .IsRequired();

        builder.Property(x => x.CaloriesMax)
            .IsRequired();

        builder.Property(x => x.ProteinMin)
            .IsRequired();

        builder.Property(x => x.ProteinMax)
            .IsRequired();

        builder.Property(x => x.CarbsMin)
            .IsRequired();

        builder.Property(x => x.CarbsMax)
            .IsRequired();

        builder.Property(x => x.FatMin)
            .IsRequired();

        builder.Property(x => x.FatMax)
            .IsRequired();

        builder.Property(x => x.TotalSugars);
        builder.Property(x => x.TotalSaturatedFats);
        builder.Property(x => x.TotalFiber);
        builder.Property(x => x.TotalSodium);
        builder.Property(x => x.TotalSalt);
        builder.Property(x => x.TotalCholesterol);

        builder.Property(x => x.Response)
            .HasMaxLength(5000)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.SavedAt);
    }
}