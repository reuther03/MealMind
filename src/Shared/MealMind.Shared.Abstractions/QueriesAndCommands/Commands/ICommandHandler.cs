using MealMind.Shared.Abstractions.Kernel.Primitives.Result;
using MealMind.Shared.Abstractions.QueriesAndCommands.Requests;

namespace MealMind.Shared.Abstractions.QueriesAndCommands.Commands;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>;