using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.Training.Domain.TrainingPlan;

public record StrengthDetails : ValueObject
{
    private readonly List<ExerciseSet> _sets = [];

    public IReadOnlyList<ExerciseSet> Sets => _sets.AsReadOnly();

    public StrengthDetails()
    {
    }

    public StrengthDetails(IEnumerable<ExerciseSet> sets)
        => _sets = [..sets];

    protected override IEnumerable<object> GetAtomicValues()
    {
        foreach (var set in Sets)
            yield return set;
    }
}