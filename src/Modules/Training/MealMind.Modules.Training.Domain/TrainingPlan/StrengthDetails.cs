using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.Training.Domain.TrainingPlan;

public record StrengthDetails : ValueObject
{
    public List<ExerciseSet> Sets { get; init; } = [];

    public void AddExerciseSet(ExerciseSet exerciseSet)
        => Sets.Add(exerciseSet);

    protected override IEnumerable<object> GetAtomicValues()
    {
        foreach (var set in Sets)
            yield return set;
    }
}