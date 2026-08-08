using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Modules.Training.Infrastructure.Database;
using MealMind.Shared.Abstractions.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MealMind.Modules.Training.Infrastructure.Seeders;

internal class ExerciseSeeder : IModuleSeeder
{
    private readonly ILogger<ExerciseSeeder> _logger;
    private readonly TrainingDbContext _dbContext;

    public ExerciseSeeder(ILogger<ExerciseSeeder> logger, TrainingDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task SeedAsync(IConfiguration configuration, CancellationToken cancellationToken)
    {
        if (await _dbContext.Exercises.AnyAsync(x => !x.IsCustom, cancellationToken))
        {
            _logger.LogInformation("Custom exercises found in the database. Skipping seeding");
            return;
        }

        _logger.LogInformation("Seeding exercises");
        var exercises = new List<Exercise>
        {
            // Strength - chest
            Exercise.Create(
                "Bench Press",
                "Lie on a flat bench, lower the bar to the middle of your chest, then press it upward until your arms are extended.",
                ExerciseType.Strength,
                MuscleGroup.Chest),
            Exercise.Create(
                "Push-Up",
                "Keep your body in a straight line, lower your chest toward the floor, then push back to the starting position.",
                ExerciseType.Strength,
                MuscleGroup.Chest),

            // Strength - back
            Exercise.Create(
                "Pull-Up",
                "Hang from a bar with an overhand grip and pull your chest toward the bar while keeping your torso controlled.",
                ExerciseType.Strength,
                MuscleGroup.Back),
            Exercise.Create(
                "Barbell Row",
                "Hinge at the hips with a neutral spine, pull the bar toward your lower ribs, then lower it under control.",
                ExerciseType.Strength,
                MuscleGroup.Back),

            // Strength - legs
            Exercise.Create(
                "Back Squat",
                "With the bar across your upper back, sit your hips down and back, then drive through your feet to stand.",
                ExerciseType.Strength,
                MuscleGroup.Legs),
            Exercise.Create(
                "Romanian Deadlift",
                "Push your hips backward while lowering the weight close to your legs, then extend your hips to return to standing.",
                ExerciseType.Strength,
                MuscleGroup.Legs),

            // Strength - shoulders
            Exercise.Create(
                "Overhead Press",
                "Press the weight from shoulder height directly overhead while keeping your core braced and ribs controlled.",
                ExerciseType.Strength,
                MuscleGroup.Shoulders),
            Exercise.Create(
                "Lateral Raise",
                "Raise the dumbbells out to your sides to shoulder height with slightly bent elbows, then lower them slowly.",
                ExerciseType.Strength,
                MuscleGroup.Shoulders),

            // Strength - biceps
            Exercise.Create(
                "Barbell Curl",
                "Keep your elbows close to your torso and curl the bar toward your shoulders without swinging your body.",
                ExerciseType.Strength,
                MuscleGroup.Biceps),
            Exercise.Create(
                "Hammer Curl",
                "With palms facing each other, curl the dumbbells toward your shoulders while keeping your upper arms still.",
                ExerciseType.Strength,
                MuscleGroup.Biceps),

            // Strength - triceps
            Exercise.Create(
                "Parallel Bar Dip",
                "Lower your body between parallel bars by bending your elbows, then press up until your arms are extended.",
                ExerciseType.Strength,
                MuscleGroup.Triceps),
            Exercise.Create(
                "Cable Triceps Pushdown",
                "Keep your elbows fixed by your sides and extend your arms downward, then return the handle under control.",
                ExerciseType.Strength,
                MuscleGroup.Triceps),

            // Strength - abs
            Exercise.Create(
                "Plank",
                "Support your body on your forearms and toes while maintaining a straight line and bracing your core.",
                ExerciseType.Strength,
                MuscleGroup.Abs),
            Exercise.Create(
                "Hanging Knee Raise",
                "Hang from a bar and lift your knees toward your chest without swinging, then lower them slowly.",
                ExerciseType.Strength,
                MuscleGroup.Abs),

            // Strength - other / full body
            Exercise.Create(
                "Farmer's Walk",
                "Carry a heavy weight in each hand while walking with an upright posture, braced core, and steady steps.",
                ExerciseType.Strength,
                MuscleGroup.Other),
            Exercise.Create(
                "Kettlebell Swing",
                "Hinge at the hips and drive them forward to swing the kettlebell to chest height without lifting it with your arms.",
                ExerciseType.Strength,
                MuscleGroup.Other),

            // Cardio
            Exercise.Create(
                "Running",
                "Run at a sustainable pace while keeping a relaxed upper body and landing softly beneath your center of mass.",
                ExerciseType.Cardio,
                null),
            Exercise.Create(
                "Stationary Cycling",
                "Cycle at a controlled cadence with the seat adjusted so your knee remains slightly bent at the bottom of each stroke.",
                ExerciseType.Cardio,
                null),

            // Other
            Exercise.Create(
                "Full-Body Mobility Flow",
                "Move smoothly through controlled hip, shoulder, and spine mobility drills without forcing the available range of motion.",
                ExerciseType.Other,
                null),
            Exercise.Create(
                "Yoga Flow",
                "Connect a sequence of basic yoga poses with slow breathing, stable alignment, and controlled transitions.",
                ExerciseType.Other,
                null)
        };

        await _dbContext.Exercises.AddRangeAsync(exercises, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
