namespace MealMind.Shared.Contracts.Dto.Training;

public class SessionStatsDto
{
    public int ExercisesCount { get; init; }
    public int TotalSets { get; init; }
    public decimal TotalVolume { get; init; }
    public decimal DurationInMinutes { get; init; }
}
