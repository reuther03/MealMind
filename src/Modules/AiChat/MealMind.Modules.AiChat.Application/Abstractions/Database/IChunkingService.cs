namespace MealMind.Modules.AiChat.Application.Abstractions.Database;

public interface IChunkingService
{
    List<string> ChunkDocument(string content, int maxTokensPerChunk = 500, int overlapTokens = 50);
}