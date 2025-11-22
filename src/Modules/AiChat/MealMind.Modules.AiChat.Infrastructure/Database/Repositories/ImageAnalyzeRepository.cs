using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Domain.ImageAnalyze;
using MealMind.Shared.Infrastructure.Postgres;

namespace MealMind.Modules.AiChat.Infrastructure.Database.Repositories;

internal class ImageAnalyzeRepository : Repository<ImageAnalyze, AiChatDbContext>, IImageAnalyzeRepository
{
    public ImageAnalyzeRepository(AiChatDbContext dbContext) : base(dbContext)
    {
    }
}