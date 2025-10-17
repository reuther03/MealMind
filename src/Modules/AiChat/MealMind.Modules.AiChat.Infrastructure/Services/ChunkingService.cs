using System.Diagnostics.CodeAnalysis;
using MealMind.Modules.AiChat.Application.Abstractions.Database;
using Microsoft.SemanticKernel.Text;

namespace MealMind.Modules.AiChat.Infrastructure.Services;

public class ChunkingService : IChunkingService
{

    public List<string> ChunkDocument(string content, int maxTokensPerChunk = 500, int overlapTokens = 50)
    {
        if (string.IsNullOrWhiteSpace(content))
            return [];

        var paragraphs = content
            .Split("\n\n", StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Trim())
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .ToList();

        var chunks = TextChunker.SplitPlainTextParagraphs(
            paragraphs,
            maxTokensPerParagraph: maxTokensPerChunk,
            overlapTokens: overlapTokens
        );

        return chunks;
    }
}