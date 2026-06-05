namespace MealMind.Shared.Contracts.Dto.Training;

public class TrainingSessionDetailsDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public DateTime? StartedAt { get; init; }
    public DateTime? EndedAt { get; init; }
    public string? Description { get; init; }
    public bool IsStarted => StartedAt.HasValue;
    public bool IsCompleted => EndedAt.HasValue;
    public List<SessionExerciseDto> Exercises { get; init; } = [];
}
