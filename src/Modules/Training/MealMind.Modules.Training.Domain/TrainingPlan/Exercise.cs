using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.Training.Domain.TrainingPlan;

public class Exercise : Entity<Guid>
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string? ImageUrl { get; private set; }
    public string? VideoUrl { get; private set; }
    public ExerciseType Type { get; private set; }
    public MuscleGroup? MuscleGroup { get; private set; }
    public bool IsCustom { get; private set; }
}