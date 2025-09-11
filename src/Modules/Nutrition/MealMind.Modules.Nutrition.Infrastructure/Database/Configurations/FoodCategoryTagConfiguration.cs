using MealMind.Modules.Nutrition.Domain.Food;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Configurations;

public class FoodCategoryTagConfiguration : IEntityTypeConfiguration<FoodCategory>
{
    public void Configure(EntityTypeBuilder<FoodCategory> builder)
    {
        builder.ToTable("FoodCategories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.FoodId)
            .HasConversion(x => x.Value, x => FoodId.From(x))
            .IsRequired();

        builder.Property(x => x.Category)
            .HasConversion<string>()
            .IsRequired();

        builder.HasIndex(x => new { x.FoodId, x.Category })
            .IsUnique();
    }
}