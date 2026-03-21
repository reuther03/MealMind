namespace MealMind.Shared.Contracts.Dto.Training;

public class ExerciseDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string? ImageUrl { get; init; }
    public string? VideoUrl { get; init; }
    public string Type { get; init; } = null!;
    public string? MuscleGroup { get; init; }
    public bool IsCustom { get; init; }
}