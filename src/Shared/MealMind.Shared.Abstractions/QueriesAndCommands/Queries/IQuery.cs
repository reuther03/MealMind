using MealMind.Shared.Abstractions.Kernel.Primitives.Result;
using MealMind.Shared.Abstractions.QueriesAndCommands.Requests;

namespace MealMind.Shared.Abstractions.QueriesAndCommands.Queries;

public interface IQueryBase;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>, IQueryBase;