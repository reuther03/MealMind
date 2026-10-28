namespace MealMind.Modules.Training.Application.Features.Caching;

public static class CacheKeyBuilder
{
    private const string TrainingPlanDetailsKeyPrefix = "training_plan_details";
    private const string TrainingSessionDetailsKeyPrefix = "training_session_details";
    private const string AllTrainingPlansKeyPrefix = "all_training_plans";
    private const string AllBaseExercisesKeyPrefix = "all_base_exercises";


    public static string GetTrainingPlanDetailsKey(Guid userId, Guid trainingPlanId)
        => $"{TrainingPlanDetailsKeyPrefix}:{userId}:{trainingPlanId}";


    public static string GetTrainingSessionDetailsKey(Guid userId, Guid trainingPlanId, Guid sessionId)
        => $"{TrainingSessionDetailsKeyPrefix}:{userId}:{trainingPlanId}:{sessionId}";

    public static string GetAllTrainingPlansKey(Guid userId)
        => $"{AllTrainingPlansKeyPrefix}:{userId}";

    public static string GetAllBaseExercisesKey()
        => $"{AllBaseExercisesKeyPrefix}";
}
