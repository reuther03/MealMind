namespace MealMind.Shared.Contracts.Dto.Training;

public class SessionExerciseDto
{
    public Guid Id { get; init; }
    public int OrderIndex { get; init; }
    public ExerciseDto Exercise { get; init; } = null!;
    public StrengthDetailsDto? StrengthDetails { get; init; }
    public CardioDetailsDto? CardioDetails { get; init; }
}

public class StrengthDetailsDto
{
    public List<ExerciseSetDto> ExerciseSets { get; init; } = [];
}

public class ExerciseSetDto
{
    public int SetNumber { get; init; }
    public int Repetitions { get; init; }
    public decimal Weight { get; init; }
    public string SetType { get; init; } = null!;
    public int? RestTimeInSeconds { get; init; }
}

public class CardioDetailsDto
{
    public int? DurationInMinutes { get; init; }
    public decimal? DistanceInKm { get; init; }
    public int? CaloriesBurned { get; init; }
    public int? AverageHeartRate { get; init; }
    public decimal? AverageSpeed { get; init; }
    public string? Notes { get; init; }
    public int? CaloriesEstimated { get; init; }
}