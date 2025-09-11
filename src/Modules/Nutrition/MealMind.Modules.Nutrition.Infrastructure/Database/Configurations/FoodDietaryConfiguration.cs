using MealMind.Modules.Nutrition.Domain.Food;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Configurations;

public class FoodDietaryConfiguration : IEntityTypeConfiguration<FoodDietaryTag>
{
    public void Configure(EntityTypeBuilder<FoodDietaryTag> builder)
    {
        builder.ToTable("FoodDietaryTags");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.FoodId)
            .HasConversion(x => x.Value, x => FoodId.From(x))
            .IsRequired();

        builder.Property(x => x.DietaryTag)
            .HasConversion<string>()
            .IsRequired();

        builder.HasIndex(x => new { x.FoodId, x.DietaryTag })
            .IsUnique();
    }
}