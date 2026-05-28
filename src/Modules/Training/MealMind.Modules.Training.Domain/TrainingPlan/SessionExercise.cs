using MealMind.Shared.Abstractions.Exception;
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

        if (strengthDetails != null && cardioDetails != null)
            throw new ArgumentException("Cannot provide both strength and cardio details — an exercise is either strength or cardio.");

        ExerciseId = exerciseId;
        OrderIndex = orderIndex;
        Notes = notes;
        StrengthDetails = strengthDetails;
        CardioDetails = cardioDetails;
    }

    public static SessionExercise Create(Guid exerciseId, int orderIndex, StrengthDetails? strengthDetails = null, CardioDetails? cardioDetails = null,
        string? notes = null)
        => new(exerciseId, orderIndex, strengthDetails, cardioDetails, notes);

    public void UpdateStrengthDetails(StrengthDetails strengthDetails)
    {
        if (CardioDetails is not null)
            throw new DomainException("Cannot set strength details on a cardio exercise.");

        StrengthDetails = strengthDetails ?? throw new ArgumentNullException(nameof(strengthDetails));
    }

    public void UpdateCardioDetails(CardioDetails cardioDetails)
    {
        if (StrengthDetails is not null)
            throw new DomainException("Cannot set cardio details on a strength exercise.");

        CardioDetails = cardioDetails ?? throw new ArgumentNullException(nameof(cardioDetails));
    }
}