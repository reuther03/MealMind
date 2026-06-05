namespace MealMind.Shared.Contracts.Dto.Training;

public class TrainingPlanDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public DayOfWeek PlannedOn { get; init; }
    public bool IsActive { get; init; }
    public int SessionsCount { get; init; }
    public DateTime? LastCompletedSessionAt { get; init; }
}
