namespace MealMind.Shared.Contracts.Dto.Training;

public class ExerciseComparisonDto
{
    public string ExerciseName { get; init; } = null!;
    public string ExerciseType { get; init; } = null!;

    public decimal? CurrentBestWeight { get; init; }
    public int? CurrentBestReps { get; init; }
    public decimal? PreviousBestWeight { get; init; }
    public int? PreviousBestReps { get; init; }
    public decimal? WeightDelta { get; init; }
    public int? RepsDelta { get; init; }

    public decimal? CurrentVolume { get; init; }
    public decimal? PreviousVolume { get; init; }
    public decimal? VolumeDelta { get; init; }

    public int? CurrentDurationInMinutes { get; init; }
    public int? PreviousDurationInMinutes { get; init; }
    public int? DurationDelta { get; init; }
    public decimal? CurrentDistanceInKm { get; init; }
    public decimal? PreviousDistanceInKm { get; init; }
    public decimal? DistanceDelta { get; init; }
}
