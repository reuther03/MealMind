using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;

namespace MealMind.Modules.Training.Domain.TrainingPlan;

public class TrainingSession : Entity<Guid>
{
    public Name Name { get; private set; } = null!;
    // public DateOnly NAME { get; private set; }

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