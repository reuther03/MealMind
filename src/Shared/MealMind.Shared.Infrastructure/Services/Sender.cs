using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.QueriesAndCommands.Notifications;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Abstractions.QueriesAndCommands.Requests;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Shared.Infrastructure.Services;

public class Sender : ISender
{
    private readonly IServiceProvider _serviceProvider;

    public Sender(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<Result<TResponse>> Send<TResponse>(IRequest<Result<TResponse>> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var responseType = typeof(TResponse);

        Type handlerInterface;

        if (typeof(ICommandBase).IsAssignableFrom(requestType))
        {
            handlerInterface = typeof(ICommandHandler<,>)
                .MakeGenericType(requestType, responseType);
        }
        else if (typeof(IQueryBase).IsAssignableFrom(requestType))
        {
            handlerInterface = typeof(IQueryHandler<,>)
                .MakeGenericType(requestType, responseType);
        }
        else if (typeof(INotification).IsAssignableFrom(requestType))
        {
            handlerInterface = typeof(INotificationHandler<>)
                .MakeGenericType(requestType);
        }
        else
        {
            throw new InvalidOperationException(
                $"Unknown request type: {requestType.Name}");
        }

        var handlerObj = _serviceProvider.GetService(handlerInterface);
        if (handlerObj == null)
        {
            throw new InvalidOperationException(
                $"No handler for {requestType.Name} found with response type {responseType.Name}."
            );
        }

        dynamic handler = handlerObj;
        return await handler.Handle((dynamic)request, cancellationToken);
    }
}