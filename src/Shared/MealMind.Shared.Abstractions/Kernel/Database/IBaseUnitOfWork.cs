using MealMind.Shared.Abstractions.Kernel.Primitives.Result;

namespace MealMind.Shared.Abstractions.Kernel.Database;

public interface IBaseUnitOfWork
{
    Task<Result> CommitAsync(CancellationToken cancellationToken = default);
}