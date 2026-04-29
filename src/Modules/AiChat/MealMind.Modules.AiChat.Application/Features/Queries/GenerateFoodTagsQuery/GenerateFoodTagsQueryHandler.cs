using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Contracts.Dto.AiChat;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.AiChat.Application.Features.Queries.GenerateFoodTagsQuery;

public class GenerateFoodTagsQueryHandler : IQueryHandler<Shared.Abstractions.Messaging.AiChat.GenerateFoodTagsQuery, FoodTagsResult>
{
    private readonly IAiChatService _aiChatService;
    private readonly IAiChatDbContext _dbContext;

    public Task<Result<FoodTagsResult>> Handle(Shared.Abstractions.Messaging.AiChat.GenerateFoodTagsQuery query, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}