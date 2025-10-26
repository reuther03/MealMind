using MealMind.Shared.Contracts.Result;

namespace MealMind.Shared.Abstractions.Kernel.Database;

public interface IBaseUnitOfWork
{
    Task<Result> CommitAsync(CancellationToken cancellationToken = default);
}