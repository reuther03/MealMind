using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Dto.AiChat;
using MealMind.Shared.Contracts.Pagination;
using MealMind.Shared.Contracts.Result;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.AiChat.Application.Features.Queries.GetConversationsQuery;

public record GetConversationsQuery(int Page = 1) : IQuery<PaginatedList<ConversationDto>>
{
    public sealed class Handler : IQueryHandler<GetConversationsQuery, PaginatedList<ConversationDto>>
    {
        private readonly IAiChatDbContext _dbContext;
        private readonly IUserService _userService;

        public Handler(IAiChatDbContext dbContext, IUserService userService)
        {
            _dbContext = dbContext;
            _userService = userService;
        }

        public async Task<Result<PaginatedList<ConversationDto>>> Handle(GetConversationsQuery query, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.AiChatUsers
                .FirstOrDefaultAsync(x => x.Id == _userService.UserId, cancellationToken);

            if (user is null)
                return Result.BadRequest<PaginatedList<ConversationDto>>("User not found.");

            var conversationsDto = await _dbContext.ChatConversations
                .Where(x => x.UserId == user.Id)
                .Skip((query.Page - 1) * 10)
                .Take(10 + 1)
                .Select(x => new ConversationDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    CreatedAt = x.CreatedAt,
                    LastUsedAt = x.LastUsedAt
                })
                .ToListAsync(cancellationToken);

            return PaginatedList<ConversationDto>.Create(query.Page, 10, conversationsDto.Count, conversationsDto.Take(10).ToList());
        }
    }
}