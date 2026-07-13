using System.Linq.Expressions;
using LinqKit;
using MealMind.Modules.AiChat.Domain.Conversation;
using MealMind.Shared.Contracts.Dto.AiChat;

namespace MealMind.Modules.AiChat.Application.Features.Mappers;

public static class ConversationMapper
{
    public static Expression<Func<Conversation, ConversationDto>> Projection { get; }
        = entity => new ConversationDto
        {
            Id = entity.Id,
            Title = entity.Title,
            CreatedAt = entity.CreatedAt,
            LastUsedAt = entity.LastUsedAt
        };

    public static Expression<Func<Conversation, ConversationDetailsDto>> DetailsProjection { get; }
        = entity => new ConversationDetailsDto
        {
            Id = entity.Id,
            Title = entity.Title,
            ChatMessages = entity.ChatMessages
                .AsQueryable()
                .Where(m => m.Role != AiChatRole.System)
                .OrderBy(m => m.CreatedAt)
                .Select(m => AiChatMessageMapper.Projection.Invoke(m))
                .ToList()
        };
}
