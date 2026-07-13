using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Application.Features.Mappers;
using MealMind.Modules.AiChat.Domain.Conversation;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Dto.AiChat;
using MealMind.Shared.Contracts.Result;
using Microsoft.EntityFrameworkCore;


namespace MealMind.Modules.AiChat.Application.Features.Queries.GetConversationQuery;

public record GetConversationQuery(Guid ChatConversationId) : IQuery<ConversationDetailsDto>
{
    public sealed class Handler : IQueryHandler<GetConversationQuery, ConversationDetailsDto>
    {
        private readonly IAiChatDbContext _dbContext;
        private readonly IUserService _userService;

        public Handler(IAiChatDbContext dbContext, IUserService userService)
        {
            _dbContext = dbContext;
            _userService = userService;
        }

        public async Task<Result<ConversationDetailsDto>> Handle(GetConversationQuery request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.AiChatUsers.FirstOrDefaultAsync(x => x.Id == _userService.UserId, cancellationToken);
            if (user == null)
                return Result<ConversationDetailsDto>.BadRequest("User not found.");

            var conversationDto = await _dbContext.ChatConversations
                .Where(x => x.UserId == user.Id &&
                    x.Id == ConversationId.From(request.ChatConversationId))
                .Select(ConversationMapper.DetailsProjection)
                .FirstOrDefaultAsync(cancellationToken);

            return conversationDto == null
                ? Result<ConversationDetailsDto>.NotFound("Conversation not found.")
                : Result<ConversationDetailsDto>.Ok(conversationDto);
        }
    }
}
