using MealMind.Shared.Abstractions.Kernel.Primitives.Result;
using MealMind.Shared.Abstractions.QueriesAndCommands.Requests;

namespace MealMind.Shared.Abstractions.Services;

public interface ISender
{
    Task<Result<TResponse>> Send<TResponse>(IRequest<Result<TResponse>> request, CancellationToken cancellationToken = default);
}