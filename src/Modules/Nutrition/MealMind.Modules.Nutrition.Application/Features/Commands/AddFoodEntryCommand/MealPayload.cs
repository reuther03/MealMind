using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;

namespace MealMind.Modules.Nutrition.Application.Features.Commands.AddFoodEntryCommand;

public record MealPayload(Guid MealId, MealType MealType, Name? Name, DateTime? ConsumedAt, string? Notes);