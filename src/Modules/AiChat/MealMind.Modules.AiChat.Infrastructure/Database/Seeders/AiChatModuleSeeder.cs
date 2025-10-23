using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Domain.Rag;
using MealMind.Shared.Abstractions.Services;
using Microsoft.Extensions.Configuration;

namespace MealMind.Modules.AiChat.Infrastructure.Database.Seeders;

public class AiChatModuleSeeder : IModuleSeeder
{
    private readonly IAiChatDbContext _dbContext;
    private readonly IChunkingService _chunkingService;
    private readonly IEmbeddingService _embeddingService;

    public AiChatModuleSeeder(IAiChatDbContext dbContext, IChunkingService chunkingService, IEmbeddingService embeddingService)
    {
        _dbContext = dbContext;
        _chunkingService = chunkingService;
        _embeddingService = embeddingService;
    }

    public async Task SeedAsync(IConfiguration configuration, CancellationToken cancellationToken)
    {
        if (_dbContext.RagDocuments.Any())
            return;

        var filesToSeed = Directory.GetFiles(@"C:\Repos\MealMind\seed-data", "*.txt");

        foreach (var file in filesToSeed)
        {
            var documentGroupId = Guid.NewGuid();

            var content = await File.ReadAllTextAsync(file, cancellationToken);
            var title = Path.GetFileNameWithoutExtension(file);

            var chunks = _chunkingService.ChunkDocument(content);
            foreach (var chunk in chunks)
            {
                var index = chunks.IndexOf(chunk);
                var embedding = await _embeddingService.GenerateEmbeddingAsync(chunk, cancellationToken);

                var ragDocument = RagDocument.Create(title, chunk, embedding, index, documentGroupId);
                await _dbContext.RagDocuments.AddAsync(ragDocument, cancellationToken);

                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}