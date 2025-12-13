using MealMind.Modules.AiChat.Domain.ImageAnalyze;
using MealMind.Shared.Abstractions.Kernel.Database;

namespace MealMind.Modules.AiChat.Application.Abstractions.Database;

public interface IImageAnalyzeRepository : IRepository<ImageAnalyzeSession>
{
    Task<ImageAnalyzeSession?> GetByIdAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default);
}