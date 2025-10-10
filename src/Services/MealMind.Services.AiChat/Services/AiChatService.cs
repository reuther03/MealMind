using MealMind.Shared.Abstractions.Services;
using Microsoft.Extensions.AI;


namespace MealMind.Services.AiChat.Services;

public class AiChatService : IAiChatService
{
    private readonly IChatClient _chatClient;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;

    public AiChatService(IChatClient chatClient, IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
    {
        _chatClient = chatClient;
        _embeddingGenerator = embeddingGenerator;
    }

    public Task<string> GetResponseAsync(string prompt, CancellationToken cancellationToken = default)
    {
        // _embeddingGenerator.GenerateAsync()\
        throw new NotImplementedException();
    }
}