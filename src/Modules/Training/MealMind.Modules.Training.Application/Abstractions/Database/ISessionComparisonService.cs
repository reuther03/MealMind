using MealMind.Shared.Contracts.Dto.Training;

namespace MealMind.Modules.Training.Application.Abstractions.Database;

public interface ISessionComparisonService
{
    Task<SessionComparisonDto> CompareSessionsAsync(Guid currentSessionId, CancellationToken cancellationToken);
}