using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Domain.ImageAnalyze;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using MealMind.Shared.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.AiChat.Infrastructure.Database.Repositories;

internal class ImageAnalyzeSessionRepository : Repository<ImageAnalyzeSession, AiChatDbContext>, IImageAnalyzeRepository
{
    private readonly AiChatDbContext _dbContext;

    public ImageAnalyzeSessionRepository(AiChatDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ImageAnalyzeSession?> GetByIdAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default)
        => await _dbContext.FoodImageAnalyzeSessions
            .FirstOrDefaultAsync(s => s.Id == ImageAnalyzeSessionId.From(sessionId) && s.UserId == UserId.From(userId), cancellationToken);
}