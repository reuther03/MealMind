using MealMind.Modules.AiChat.Application.Dtos;
using MealMind.Shared.Abstractions.Kernel.Pagination;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.AiChat.Application.Features.Queries.GetConversationsQuery;

public class GetConversationsQuery : IQuery<PaginatedList<ConversationDto>>
{
    public sealed class Handler : IQueryHandler<GetConversationsQuery, PaginatedList<ConversationDto>>
    {
        public Task<Result<PaginatedList<ConversationDto>>> Handle(GetConversationsQuery query, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}