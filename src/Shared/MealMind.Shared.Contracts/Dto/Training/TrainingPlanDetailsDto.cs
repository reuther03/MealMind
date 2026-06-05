namespace MealMind.Shared.Contracts.Dto.Training;

public class TrainingPlanDetailsDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public DayOfWeek PlannedOn { get; init; }
    public bool IsActive { get; init; }
    public List<TrainingSessionDto> Sessions { get; init; } = [];
}