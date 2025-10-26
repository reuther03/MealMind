using MealMind.Shared.Abstractions.QueriesAndCommands.Requests;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Shared.Abstractions.Services;

public interface ISender
{
    Task<Result<TResponse>> Send<TResponse>(IRequest<Result<TResponse>> request, CancellationToken cancellationToken = default);
}