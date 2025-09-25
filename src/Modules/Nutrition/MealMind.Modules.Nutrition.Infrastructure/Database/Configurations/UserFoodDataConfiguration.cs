using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Modules.Nutrition.Domain.UserProfile;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Configurations;

public class UserFoodDataConfiguration : IEntityTypeConfiguration<UserFoodData>
{
    public void Configure(EntityTypeBuilder<UserFoodData> builder)
    {
        builder.ToTable("UserFoodData");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.FoodId)
            .HasConversion(x => x.Value, x => FoodId.From(x))
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasConversion(x => x.Value, x => UserId.From(x))
            .IsRequired();

        builder.Property(x => x.Rating)
            .IsRequired(false);

        builder.Property(x => x.IsFavorite)
            .IsRequired();

        builder.Property(x => x.TimesUsed)
            .IsRequired();

        builder.Property(x => x.LastUsedAt)
            .IsRequired(false);

        builder.HasIndex(x => new { x.UserId, x.FoodId })
            .IsUnique();

        builder.HasOne<Food>()
            .WithMany()
            .HasForeignKey(x => x.FoodId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}