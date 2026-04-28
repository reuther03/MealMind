using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Contracts.Dto.AiChat;

namespace MealMind.Shared.Abstractions.Messaging.AiChat;

public class GenerateFoodTagsQuery : IQuery<FoodTagsResult>;