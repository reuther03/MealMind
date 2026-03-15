using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.Training.Domain.TrainingPlan;

public record ExerciseSet : ValueObject
{
    public int SetNumber { get; init; }
    public int Repetitions { get; init; }
    public decimal Weight { get; init; }
    public SetType SetType { get; init; }
    public int? RestTimeInSeconds { get; init; }

    public ExerciseSet(int setNumber, int repetitions, decimal weight, SetType setType, int? restTimeInSeconds = null)
    {
        SetNumber = setNumber;
        Repetitions = repetitions;
        Weight = weight;
        SetType = setType;
        RestTimeInSeconds = restTimeInSeconds;
    }


    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return SetNumber;
        yield return Repetitions;
        yield return Weight;
        yield return SetType;
        yield return RestTimeInSeconds ?? 0;
    }
}