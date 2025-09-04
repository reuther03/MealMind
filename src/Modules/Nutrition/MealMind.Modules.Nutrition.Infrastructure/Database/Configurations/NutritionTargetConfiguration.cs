using MealMind.Modules.Nutrition.Domain.UserProfile;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Configurations;

public class NutritionTargetConfiguration : IEntityTypeConfiguration<NutritionTarget>
{
    public void Configure(EntityTypeBuilder<NutritionTarget> builder)
    {
        builder.ToTable("NutritionTarget");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Calories)
            .IsRequired();

        builder.Property(x => x.Protein)
            .IsRequired();

        builder.Property(x => x.Carbohydrates)
            .IsRequired();

        builder.Property(x => x.Fats)
            .IsRequired();

        builder.Property(x => x.WaterIntake)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.DeactivatedAt)
            .IsRequired(false);

        builder.Property(x => x.UserProfileId)
            .HasConversion(x => x.Value, x => UserId.From(x))
            .IsRequired();
    }
}