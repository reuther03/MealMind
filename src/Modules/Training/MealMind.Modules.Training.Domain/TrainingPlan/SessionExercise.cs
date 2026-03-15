using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.Training.Domain.TrainingPlan;

public class SessionExercise : Entity<Guid>
{
    public Guid ExerciseId { get; private set; }
    public int OrderIndex { get; private set; }
    public StrengthDetails? StrengthDetails { get; private set; }
    public CardioDetails? CardioDetails { get; private set; }
    public string? Notes { get; private set; }
}