using MealMind.Shared.Abstractions.Exception;
using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.Training.Domain.TrainingPlan;

public record CardioDetails : ValueObject
{
    public int? DurationInMinutes { get; init; }
    public decimal? DistanceInKm { get; init; }
    public int? CaloriesBurned { get; init; }
    public int? AverageHeartRate { get; init; }
    public decimal? AverageSpeed { get; init; }
    public string? Notes { get; init; }
    public int? CaloriesEstimated { get; init; }

    private CardioDetails(
        int? durationInMinutes,
        decimal? distanceInKm,
        int? caloriesBurned,
        int? averageHeartRate,
        decimal? averageSpeed,
        string? notes,
        int? caloriesEstimated
    )
    {
        if (durationInMinutes is <= 0)
            throw new DomainException("Duration must be a positive integer.", nameof(durationInMinutes));

        if (distanceInKm is <= 0)
            throw new DomainException("Distance must be a positive integer.", nameof(distanceInKm));

        if (caloriesBurned is <= 0)
            throw new DomainException("Calories burned must be a positive integer.", nameof(caloriesBurned));

        if (averageHeartRate is <= 0)
            throw new DomainException("Average heart rate must be a positive integer.", nameof(averageHeartRate));

        if (averageSpeed is <= 0)
            throw new DomainException("Average speed must be a positive integer.", nameof(averageSpeed));

        DurationInMinutes = durationInMinutes;
        DistanceInKm = distanceInKm;
        CaloriesBurned = caloriesBurned;
        AverageHeartRate = averageHeartRate;
        AverageSpeed = averageSpeed;
        Notes = notes;
        CaloriesEstimated = caloriesEstimated;
    }

    public static CardioDetails CreateEmpty() => new(null, null, null, null, null, null, null);


    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return DurationInMinutes ?? 0;
        yield return DistanceInKm ?? 0;
        yield return CaloriesBurned ?? 0;
        yield return AverageHeartRate ?? 0;
        yield return AverageSpeed ?? 0;
        yield return Notes ?? string.Empty;
        yield return CaloriesEstimated ?? 0;
    }
}