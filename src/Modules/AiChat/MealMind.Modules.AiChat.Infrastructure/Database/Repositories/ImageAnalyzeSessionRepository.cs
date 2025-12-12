using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Domain.ImageAnalyze;
using MealMind.Shared.Infrastructure.Postgres;

namespace MealMind.Modules.AiChat.Infrastructure.Database.Repositories;

internal class ImageAnalyzeSessionRepository : Repository<ImageAnalyzeSession, AiChatDbContext>, IImageAnalyzeRepository
{
    private readonly AiChatDbContext _dbContext;

    public ImageAnalyzeSessionRepository(AiChatDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ImageAnalyzeSession?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
        => await _dbContext.FoodImageAnalyzeSessions.FindAsync([sessionId], cancellationToken);
}