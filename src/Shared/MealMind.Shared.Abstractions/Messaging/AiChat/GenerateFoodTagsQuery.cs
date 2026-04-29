using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Contracts.Dto.AiChat;

namespace MealMind.Shared.Abstractions.Messaging.AiChat;

public record GenerateFoodTagsQuery(string ProductName, string? Brand) : IQuery<FoodTagsResult>;