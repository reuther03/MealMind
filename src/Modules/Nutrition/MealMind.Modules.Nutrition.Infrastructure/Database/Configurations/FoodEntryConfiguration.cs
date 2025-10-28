using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Configurations;

public class FoodEntryConfiguration : IEntityTypeConfiguration<FoodEntry>
{
    public void Configure(EntityTypeBuilder<FoodEntry> builder)
    {
        builder.ToTable("FoodEntry");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.FoodId)
            .HasConversion(x => x.Value, x => FoodId.From(x))
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.FoodName)
            .HasConversion(x => x.Value, x => new Name(x))
            .IsRequired();

        builder.Property(x => x.FoodBrand)
            .HasConversion(x => x!.Value, x => new Name(x))
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.QuantityInGrams)
            .HasPrecision(8, 2)
            .IsRequired();

        builder.Property(x => x.TotalCalories)
            .HasPrecision(10, 2)  // Increased to support large quantities
            .IsRequired();

        builder.Property(x => x.TotalProteins)
            .HasPrecision(8, 2)   // Increased to support large quantities
            .IsRequired();

        builder.Property(x => x.TotalCarbohydrates)
            .HasPrecision(8, 2)   // Increased to support large quantities
            .IsRequired();

        builder.Property(x => x.TotalSugars)
            .HasPrecision(8, 2)   // Increased to support large quantities
            .IsRequired(false);

        builder.Property(x => x.TotalFats)
            .HasPrecision(8, 2)   // Increased to support large quantities
            .IsRequired();

        builder.Property(x => x.TotalSaturatedFats)
            .HasPrecision(8, 2)   // Increased to support large quantities
            .IsRequired(false);

        builder.Property(x => x.TotalFiber)
            .HasPrecision(8, 2)   // Increased to support large quantities
            .IsRequired(false);

        builder.Property(x => x.TotalSodium)
            .HasPrecision(10, 2)  // Increased to support sodium in mg
            .IsRequired(false);

        builder.Property(x => x.TotalSalt)
            .HasPrecision(10, 2)  // Increased to support salt in mg
            .IsRequired(false);

        builder.Property(x => x.TotalCholesterol)
            .HasPrecision(8, 2)   // Increased to support large quantities
            .IsRequired(false);
    }
}