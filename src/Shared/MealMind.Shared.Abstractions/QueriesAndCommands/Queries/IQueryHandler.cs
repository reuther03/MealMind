using MealMind.Shared.Contracts.Result;

namespace MealMind.Shared.Abstractions.QueriesAndCommands.Queries;

public interface IQueryHandler<in TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken = default);
}