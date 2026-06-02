using MealMind.Shared.Abstractions.Exception;
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
        if (restTimeInSeconds <= 0)
            throw new DomainException("Rest time must be a positive integer.", nameof(restTimeInSeconds));

        if (repetitions <= 0)
            throw new DomainException("Repetitions must be a positive integer.", nameof(repetitions));

        if (weight < 0)
            throw new DomainException("Weight must be a positive integer.", nameof(weight));

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