using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.Training.Domain.TrainingPlan;

public class SessionExercise : Entity<Guid>
{
    public Guid ExerciseId { get; private set; }
    public int OrderIndex { get; private set; }
    public string? Notes { get; private set; }
    public StrengthDetails? StrengthDetails { get; private set; }
    public CardioDetails? CardioDetails { get; private set; }

    private SessionExercise()
    {
    }

    private SessionExercise(Guid exerciseId, int orderIndex, StrengthDetails? strengthDetails, CardioDetails? cardioDetails, string? notes)
    {
        if (strengthDetails == null && cardioDetails == null)
            throw new ArgumentException("Either strength details or cardio details must be provided.");

        ExerciseId = exerciseId;
        OrderIndex = orderIndex;
        Notes = notes;
        StrengthDetails = strengthDetails;
        CardioDetails = cardioDetails;
    }

    public static SessionExercise Create(Guid exerciseId, int orderIndex, StrengthDetails? strengthDetails = null, CardioDetails? cardioDetails = null,
        string? notes = null)
        => new(exerciseId, orderIndex, strengthDetails, cardioDetails, notes);

    public void SetStrengthDetails(StrengthDetails? strengthDetails)
        => StrengthDetails = strengthDetails;
}