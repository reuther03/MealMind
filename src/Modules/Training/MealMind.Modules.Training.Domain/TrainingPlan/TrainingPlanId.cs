using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.Training.Domain.TrainingPlan;

public record TrainingPlanId : EntityId
{
    private TrainingPlanId(Guid value) : base(value)
    {
    }

    public static TrainingPlanId New() => new(Guid.NewGuid());
    public static TrainingPlanId From(Guid value) => new(value);
    public static TrainingPlanId From(string value) => new(Guid.Parse(value));

    public static implicit operator Guid(TrainingPlanId id) => id.Value;
    public static implicit operator TrainingPlanId(Guid id) => new(id);

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
