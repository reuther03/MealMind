using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Modules.Nutrition.Domain.UserProfile;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Configurations;

public class FoodStatisticsConfiguration : IEntityTypeConfiguration<FoodStatistics>
{
    public void Configure(EntityTypeBuilder<FoodStatistics> builder)
    {
        builder.ToTable("FoodStatistics");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.FoodId)
            .HasConversion(x => x.Value, x => FoodId.From(x))
            .IsRequired();

        builder.Property(x => x.TotalUsageCount)
            .IsRequired();

        builder.Property(x => x.FavoriteCount)
            .IsRequired();

        builder.Property(x => x.AverageRating)
            .HasPrecision(3, 2)
            .IsRequired();

        builder.Property(x => x.RatingCount)
            .IsRequired();

        builder.Property(x => x.LastUsedAt)
            .IsRequired(false);

        builder.Property(x => x.SearchCount)
            .IsRequired();

        builder.Property(x => x.PopularityScore)
            .HasPrecision(10, 4)
            .IsRequired();

        builder.Property(x => x.WeightedRating)
            .HasPrecision(10, 4)
            .HasComputedColumnSql()
            .IsRequired();

        // Unique index on FoodId - one statistics record per food
        builder.HasIndex(x => x.FoodId)
            .IsUnique();

        // Index for sorting by popularity or { TotalUsageCount, FavoriteCount }
        // builder.HasIndex(x => x.TotalUsageCount);
        // builder.HasIndex(x => x.FavoriteCount);

        builder.HasOne<Food>()
            .WithMany()
            .HasForeignKey(x => x.FoodId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}