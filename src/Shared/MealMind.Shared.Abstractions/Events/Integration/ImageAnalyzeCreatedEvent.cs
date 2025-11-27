using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using MealMind.Shared.Abstractions.QueriesAndCommands.Notifications;

namespace MealMind.Shared.Abstractions.Events.Integration;

public record ImageAnalyzeCreatedEvent(
    UserId UserId,
    Name FoodName,
    decimal QuantityInGrams,
    decimal TotalCalories,
    decimal TotalProteins,
    decimal TotalCarbohydrates,
    decimal TotalFats,
    DateOnly DailyLogDate
) : INotification;