using MealMind.Shared.Abstractions.Events.Core;
using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Contracts.Dto.Nutrition;

namespace MealMind.Shared.Events.AiChat;

public record FoodCreatedEvent(FoodDto Food) : Event(Guid.NewGuid());