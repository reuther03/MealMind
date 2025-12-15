using MealMind.Shared.Abstractions.Events.Core;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Shared.Events.AiChat;

public record ImageAnalyzeCreatedEvent(
    UserId UserId,
    string FoodName,
    decimal QuantityInGrams,
    decimal TotalCalories,
    decimal TotalProteins,
    decimal TotalCarbohydrates,
    decimal TotalFats,
    DateOnly DailyLogDate
) : Event(Guid.NewGuid());