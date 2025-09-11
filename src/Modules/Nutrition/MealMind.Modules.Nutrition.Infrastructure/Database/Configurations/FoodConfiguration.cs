using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Configurations;

public class FoodConfiguration : IEntityTypeConfiguration<Food>
{
    public void Configure(EntityTypeBuilder<Food> builder)
    {
        builder.ToTable("Food");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => FoodId.From(x))
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasConversion(x => x.Value, x => new Name(x))
            .IsRequired();

        builder.Property(x => x.Barcode)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.OwnsOne(x => x.NutritionPer100G, npg =>
        {
            npg.ToTable("NutritionPer100G");

            npg.WithOwner()
                .HasForeignKey("FoodId");

            npg.HasKey("FoodId");

            npg.Property(x => x.Calories)
                .HasPrecision(8, 2)
                .IsRequired();

            npg.Property(x => x.Protein)
                .HasPrecision(6, 2)
                .IsRequired();

            npg.Property(x => x.Fat)
                .HasPrecision(6, 2)
                .IsRequired();

            npg.Property(x => x.Carbohydrates)
                .HasPrecision(6, 2)
                .IsRequired();

            npg.Property(x => x.Sugar)
                .HasPrecision(6, 2);

            npg.Property(x => x.SaturatedFat)
                .HasPrecision(6, 2);

            npg.Property(x => x.Fiber)
                .HasPrecision(6, 2);

            npg.Property(x => x.Sodium)
                .HasPrecision(8, 2);

            npg.Property(x => x.Cholesterol)
                .HasPrecision(6, 2);
        });

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(2048)
            .IsRequired(false);

        builder.Property(x => x.Brand)
            .HasMaxLength(300)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.Source)
            .HasConversion<string>()
            .IsRequired();

        builder.HasMany(x => x.Categories)
            .WithOne()
            .HasForeignKey(x => x.FoodId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.DietaryTags)
            .WithOne()
            .HasForeignKey(x => x.FoodId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Barcode).IsUnique();
    }
}