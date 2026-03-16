using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealMind.Modules.Training.Infrastructure.Database.Configurations;

public class TrainingSessionConfiguration : IEntityTypeConfiguration<TrainingSession>
{
    public void Configure(EntityTypeBuilder<TrainingSession> builder)
    {
        builder.ToTable("TrainingSession");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasConversion(x => x.Value, x => new Name(x))
            .IsRequired();

        builder.Property(x => x.StartedAt);
        builder.Property(x => x.EndedAt);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.HasMany(x => x.Exercises)
            .WithOne()
            .HasForeignKey("TrainingSessionId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}