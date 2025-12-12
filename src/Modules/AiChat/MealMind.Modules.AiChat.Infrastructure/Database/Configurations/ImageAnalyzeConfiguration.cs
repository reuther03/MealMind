using MealMind.Modules.AiChat.Domain.ImageAnalyze;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealMind.Modules.AiChat.Infrastructure.Database.Configurations;

public class ImageAnalyzeConfiguration : IEntityTypeConfiguration<ImageAnalyze>
{
    public void Configure(EntityTypeBuilder<ImageAnalyze> builder)
    {
        builder.ToTable("ImageAnalyze");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.SessionId)
            .IsRequired(false);

        builder.Property(x => x.FoodName)
            .HasMaxLength(1000)
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

        builder.Property(x => x.ConfidenceScore)
            .IsRequired();

        builder.Property(x => x.TotalQuantityInGrams)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.SavedAt);
    }
}