using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Configurations;

public class MealConfiguration : IEntityTypeConfiguration<Meal>
{
    public void Configure(EntityTypeBuilder<Meal> builder)
    {
        builder.ToTable("Meal");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.MealType)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.Name)
            .HasConversion(x => x!.Value, x => new Name(x))
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasConversion(x => x.Value, x => UserId.From(x))
            .IsRequired();

        builder.Property(x => x.ConsumedAt)
            .IsRequired(false);

        builder.Property(x => x.Notes)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.HasMany(x => x.Foods)
            .WithOne()
            .HasForeignKey("MealId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}