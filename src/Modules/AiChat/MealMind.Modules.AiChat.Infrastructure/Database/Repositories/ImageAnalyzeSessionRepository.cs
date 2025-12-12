using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Domain.ImageAnalyze;
using MealMind.Shared.Infrastructure.Postgres;

namespace MealMind.Modules.AiChat.Infrastructure.Database.Repositories;

internal class ImageAnalyzeSessionRepository : Repository<ImageAnalyzeSession, AiChatDbContext>, IImageAnalyzeRepository
{
    public ImageAnalyzeSessionRepository(AiChatDbContext dbContext) : base(dbContext)
    {
    }
}