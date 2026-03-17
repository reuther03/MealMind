using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealMind.Modules.Training.Infrastructure.Database.Configurations;

public class TrainingPlanConfiguration : IEntityTypeConfiguration<TrainingPlan>
{
    public void Configure(EntityTypeBuilder<TrainingPlan> builder)
    {
        builder.ToTable("TrainingPlan");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => TrainingPlanId.From(x))
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasConversion(x => x.Value, x => new Name(x))
            .IsRequired();

        builder.Property(x => x.PlannedOn)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasConversion(x => x.Value, x => UserId.From(x))
            .ValueGeneratedNever()
            .IsRequired();

        builder.HasMany(x => x.Sessions)
            .WithOne()
            .HasForeignKey("TrainingPlanId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}