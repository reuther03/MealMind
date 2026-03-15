using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.Training.Domain.TrainingPlan;

public class Exercise : Entity<Guid>
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string? ImageUrl { get; private set; }
    public string? VideoUrl { get; private set; }
    public ExerciseType Type { get; private set; }
    public MuscleGroup? MuscleGroup { get; private set; }
    public bool IsCustom { get; private set; }

    private Exercise()
    {
    }

    private Exercise(string name, string description, ExerciseType type, MuscleGroup? muscleGroup, bool isCustom, string? imageUrl, string? videoUrl)
    {
        if (type == ExerciseType.Strength && muscleGroup == null)
            throw new ArgumentException("Muscle group must be specified for strength exercises.", nameof(muscleGroup));

        Name = name;
        Description = description;
        Type = type;
        MuscleGroup = muscleGroup;
        IsCustom = isCustom;
        ImageUrl = imageUrl;
        VideoUrl = videoUrl;
    }

    public static Exercise Create(
        string name,
        string description,
        ExerciseType type,
        MuscleGroup? muscleGroup,
        bool isCustom = false,
        string? imageUrl = null,
        string? videoUrl = null
    ) => new(name, description, type, muscleGroup, isCustom, imageUrl, videoUrl);
}