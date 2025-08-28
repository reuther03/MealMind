using MealMind.Shared.Abstractions.Kernel.Primitives.Result;

namespace MealMind.Shared.Abstractions.QueriesAndCommands.Requests;

public interface IRequestHandler<in TCommand, TResult> where TCommand : IRequest<Result>
{
    Task<TResult> Handle(TCommand command, CancellationToken cancellationToken);
}