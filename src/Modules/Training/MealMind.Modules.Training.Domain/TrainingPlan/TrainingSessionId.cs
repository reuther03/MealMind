using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.Training.Domain.TrainingPlan;

public record TrainingSessionId : EntityId
{
    private TrainingSessionId(Guid value) : base(value)
    {
    }

    public static TrainingSessionId New() => new(Guid.NewGuid());
    public static TrainingSessionId From(Guid value) => new(value);
    public static TrainingSessionId From(string value) => new(Guid.Parse(value));

    public static implicit operator Guid(TrainingSessionId id) => id.Value;
    public static implicit operator TrainingSessionId(Guid id) => new(id);

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
