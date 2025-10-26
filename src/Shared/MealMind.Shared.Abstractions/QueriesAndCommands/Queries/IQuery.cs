using MealMind.Shared.Abstractions.QueriesAndCommands.Requests;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Shared.Abstractions.QueriesAndCommands.Queries;

public interface IQueryBase;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>, IQueryBase;