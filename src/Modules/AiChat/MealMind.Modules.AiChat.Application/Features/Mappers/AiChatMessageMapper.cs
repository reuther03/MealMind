using System.Linq.Expressions;
using MealMind.Modules.AiChat.Domain.Conversation;
using MealMind.Shared.Contracts.Dto.AiChat;

namespace MealMind.Modules.AiChat.Application.Features.Mappers;

public static class AiChatMessageMapper
{
    public static Expression<Func<AiChatMessage, AiChatMessageDto>> Projection { get; }
        = entity => new AiChatMessageDto
        {
            Id = entity.Id,
            Role = entity.Role.ToString(),
            Content = entity.Content,
            ReplyToMessageId = entity.ReplyToMessageId,
            CreatedAt = entity.CreatedAt
        };
}
