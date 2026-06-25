namespace MealMind.Shared.Contracts.Dto.Training;

public class SessionComparisonDto
{
    public Guid SessionId { get; init; }
    public Guid? PreviousSessionId { get; init; }
    public DateTime EndedAt { get; init; }
    public DateTime? PreviousEndedAt { get; init; }
    public SessionStatsDto Current { get; init; } = null!;
    public SessionStatsDto? Previous { get; init; }
    public List<ExerciseComparisonDto> Exercises { get; init; } = [];
}
