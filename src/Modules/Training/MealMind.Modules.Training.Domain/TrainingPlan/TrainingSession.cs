using MealMind.Shared.Abstractions.Exception;
using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;

namespace MealMind.Modules.Training.Domain.TrainingPlan;

public class TrainingSession : Entity<Guid>
{
    private readonly List<SessionExercise> _exercises = [];

    public Name Name { get; private set; } = null!;
    public DateTime? StartedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }
    public string? Description { get; private set; }
    public bool IsCompleted => EndedAt.HasValue;

    public IReadOnlyList<SessionExercise> Exercises => _exercises.AsReadOnly();


    private TrainingSession()
    {
    }

    private TrainingSession(Name name, string? description)
    {
        Name = name;
        Description = description;
    }

    public static TrainingSession Create(Name name, string? description = null)
        => new(name, description);

    public void SetAsCompleted()
    {
        if (IsCompleted)
            throw new DomainException("Training session is already completed.");

        EndedAt = DateTime.UtcNow;
    }

    public void AddExercise(SessionExercise exercise)
    {
        if (IsCompleted)
            throw new DomainException("Training session is already completed.");

        _exercises.Add(exercise);
    }
}