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
        DurationInMinutes = durationInMinutes;
        DistanceInKm = distanceInKm;
        CaloriesBurned = caloriesBurned;
        AverageHeartRate = averageHeartRate;
        AverageSpeed = averageSpeed;
        Notes = notes;
        CaloriesEstimated = caloriesEstimated;
    }

    public static CardioDetails CreateEmpty()
        => new CardioDetails(null, null, null, null, null, null, null);


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