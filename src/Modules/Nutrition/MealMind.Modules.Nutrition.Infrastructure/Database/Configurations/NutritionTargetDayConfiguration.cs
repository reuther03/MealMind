using MealMind.Modules.Nutrition.Domain.UserProfile;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Configurations;

public class NutritionTargetDayConfiguration : IEntityTypeConfiguration<NutritionTargetActiveDays>
{
    public void Configure(EntityTypeBuilder<NutritionTargetActiveDays> builder)
    {
        builder.ToTable("NutritionTargetActiveDays");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.NutritionTargetId)
            .IsRequired();

        builder.Property(x => x.DayOfWeek)
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();

        // Ensure unique day per nutrition target
        builder.HasIndex(x => new { x.NutritionTargetId, x.DayOfWeek })
            .IsUnique();
    }
}