using MealMind.Shared.Abstractions.Kernel.Primitives.Result;

namespace MealMind.Shared.Abstractions.QueriesAndCommands.Queries;

public interface IQueryHandler<in TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken = default);
}