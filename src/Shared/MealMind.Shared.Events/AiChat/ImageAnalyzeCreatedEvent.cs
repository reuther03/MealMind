using MealMind.Shared.Abstractions.Events.Core;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using MealMind.Shared.Abstractions.QueriesAndCommands.Notifications;

namespace MealMind.Shared.Events.AiChat;

public record ImageAnalyzeCreatedEvent(
    Guid UserId,
    string FoodName,
    decimal QuantityInGrams,
    decimal TotalCalories,
    decimal TotalProteins,
    decimal TotalCarbohydrates,
    decimal TotalFats,
    DateOnly DailyLogDate
) : Event(Guid.NewGuid());