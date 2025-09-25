using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Configurations;

public class FoodReviewConfiguration : IEntityTypeConfiguration<FoodReview>
{
    public void Configure(EntityTypeBuilder<FoodReview> builder)
    {
        builder.ToTable("FoodReviews");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.FoodId)
            .HasConversion(x => x.Value, x => FoodId.From(x))
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasConversion(x => x.Value, x => UserId.From(x))
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Rating)
            .IsRequired();

        builder.Property(x => x.Comment)
            .HasMaxLength(250)
            .IsRequired(false);

        builder.HasIndex(x => new { x.FoodId, x.UserId })
            .IsUnique();

        builder.HasOne<Food>()
            .WithMany()
            .HasForeignKey(x => x.FoodId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}