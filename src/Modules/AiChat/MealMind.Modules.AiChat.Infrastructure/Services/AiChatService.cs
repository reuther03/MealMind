using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Domain.Conversation;
using MealMind.Modules.AiChat.Infrastructure.Database;
using MealMind.Shared.Abstractions.Kernel.CommandValidators;
using MealMind.Shared.Abstractions.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;

namespace MealMind.Modules.AiChat.Infrastructure.Services;

public class AiChatService : IAiChatService
{
    private readonly IChatClient _chatClient;
    private readonly IAiChatDbContext _dbContext;

    public AiChatService(IChatClient chatClient, IAiChatDbContext dbContext)
    {
        _chatClient = chatClient;
        _dbContext = dbContext;
    }

    public async Task<string> GetResponseAsync(string prompt, Guid conversationId, CancellationToken cancellationToken = default)
    {
        var conversation = await _dbContext.ChatConversations
            .Include(x => x.ChatMessages)
            .FirstOrDefaultAsync(x => x.Id.Value == conversationId, cancellationToken);

        NullValidator.ValidateNotNull(conversation);

        var chatMessages = conversation.ChatMessages
            .Select(x => new ChatMessage(new ChatRole(x.Role.ToString()), x.Content))
            .ToList();

        NullValidator.ValidateNotNull(chatMessages);

        var userMessage = new ChatMessage(new ChatRole("user"), prompt);
        var aiChatMessage = AiChatMessage.Create(conversation.Id, AiChatRole.User, prompt, conversation.GetRecentMessage().Id);

        chatMessages.Add(userMessage);
        conversation.AddMessage(aiChatMessage);

        var response = await _chatClient.GetResponseAsync(chatMessages, cancellationToken: cancellationToken);

        var assistantMessage = AiChatMessage.Create(conversation.Id, AiChatRole.Assistant, response.Text, aiChatMessage.Id);
        conversation.AddMessage(assistantMessage);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return response.Text;
    }
}