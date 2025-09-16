using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Configurations;

public class DailyLogConfiguration : IEntityTypeConfiguration<DailyLog>
{
    public void Configure(EntityTypeBuilder<DailyLog> builder)
    {
        builder.ToTable("DailyLog");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => DailyLogId.From(x))
            .ValueGeneratedNever();

        builder.Property(x => x.CurrentDate)
            .IsRequired();

        builder.Property(x => x.CurrentWeight)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(x => x.CaloriesGoal)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasConversion(x => x.Value, x => UserId.From(x))
            .IsRequired();

        builder.HasMany(x => x.Meals)
            .WithOne()
            .HasForeignKey("DailyLogId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}