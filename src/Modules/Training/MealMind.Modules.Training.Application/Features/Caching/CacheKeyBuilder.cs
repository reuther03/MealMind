namespace MealMind.Modules.Training.Application.Features.Caching;

public static class CacheKeyBuilder
{
    private const string TrainingPlanDetailsKeyPrefix = "training_plan_details";

    public static string GetTrainingPlanDetailsKey(Guid userId, Guid trainingPlanId)
    {
        return $"{TrainingPlanDetailsKeyPrefix}:{userId}:{trainingPlanId}";
    }
}
