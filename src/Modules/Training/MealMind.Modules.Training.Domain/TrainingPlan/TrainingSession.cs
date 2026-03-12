using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;

namespace MealMind.Modules.Training.Domain.TrainingPlan;

public class TrainingSession : Entity<Guid>
{
    private readonly List<Exercise> _exercises = new();

    public Name Name { get; private set; } = null!;
    public DateTime StartedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }
    public string? Description { get; private set; }
    public bool IsCompleted => EndedAt.HasValue;

    public IReadOnlyList<Exercise> Exercises => _exercises.AsReadOnly();


    private TrainingSession()
    {
    }

    private TrainingSession(Guid id, Name name) : base(id)
    {
        Name = name;
    }

    public static TrainingSession Create(Name name) =>
        new(Guid.NewGuid(), name);
}