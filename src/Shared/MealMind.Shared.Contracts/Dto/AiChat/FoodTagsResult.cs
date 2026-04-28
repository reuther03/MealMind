namespace MealMind.Shared.Contracts.Dto.AiChat;

public sealed record FoodTagsResult(
    IReadOnlyList<string> Categories,
    IReadOnlyList<string> DietaryTags);