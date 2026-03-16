using MealMind.Modules.Training.Domain.TrainingPlan;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealMind.Modules.Training.Infrastructure.Database.Configurations;

public class SessionExerciseConfiguration : IEntityTypeConfiguration<SessionExercise>
{
    public void Configure(EntityTypeBuilder<SessionExercise> builder)
    {
        builder.ToTable("SessionExercise");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.ExerciseId)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.OrderIndex)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.OwnsOne(x => x.StrengthDetails, sd =>
        {
            sd.ToJson();
            sd.OwnsMany(x => x.Sets);
        });

        builder.OwnsOne(x => x.CardioDetails, sd =>
        {
            sd.ToJson();
        });
    }
}