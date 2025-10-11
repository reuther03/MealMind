using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Infrastructure.Database;
using MealMind.Shared.Abstractions.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

        throw new NotImplementedException();
    }
}