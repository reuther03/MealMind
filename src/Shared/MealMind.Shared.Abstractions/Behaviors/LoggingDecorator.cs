using System.Diagnostics;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.QueriesAndCommands.Notifications;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Contracts.Result;
using Microsoft.Extensions.Logging;

namespace MealMind.Shared.Abstractions.Behaviors;

public class LoggingDecorator
{
    [Decorator]
    public sealed class QueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        private readonly ILogger<QueryHandler<TQuery, TResponse>> _logger;
        private readonly IQueryHandler<TQuery, TResponse> _innerHandler;

        public QueryHandler(ILogger<QueryHandler<TQuery, TResponse>> logger, IQueryHandler<TQuery, TResponse> innerHandler)
        {
            _logger = logger;
            _innerHandler = innerHandler;
        }

        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken = default)
        {
            var requestName = typeof(TQuery).Name;

            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation(
                "{Timestamp} | Handling {RequestName}",
                DateTime.UtcNow,
                requestName);

            Result<TResponse> result = await _innerHandler.Handle(query, cancellationToken);

            if (result.IsSuccess)
            {
                stopwatch.Stop();
                _logger.LogInformation(
                    "{Timestamp} | Handled successfully {RequestName} | {ElapsedMilliseconds}ms",
                    DateTime.UtcNow,
                    requestName,
                    stopwatch.ElapsedMilliseconds);
            }
            else
            {
                stopwatch.Stop();
                _logger.LogError(
                    "{Timestamp} | {RequestName} Error handling | {ElapsedMilliseconds}ms | {ResultMessage}",
                    DateTime.UtcNow,
                    requestName,
                    stopwatch.ElapsedMilliseconds,
                    result.Message
                );
            }

            return result;
        }
    }

    [Decorator]
    public sealed class CommandHandler<TCommand, TResponse> : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        private readonly ILogger<CommandHandler<TCommand, TResponse>> _logger;
        private readonly ICommandHandler<TCommand, TResponse> _innerHandler;

        public CommandHandler(ILogger<CommandHandler<TCommand, TResponse>> logger, ICommandHandler<TCommand, TResponse> innerHandler)
        {
            _logger = logger;
            _innerHandler = innerHandler;
        }

        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken = default)
        {
            var requestName = typeof(TCommand).Name;

            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation(
                "{Timestamp} | Handling {RequestName}",
                DateTime.UtcNow,
                requestName);

            Result<TResponse> result = await _innerHandler.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                stopwatch.Stop();
                _logger.LogInformation(
                    "{Timestamp} | Handled successfully {RequestName} | {ElapsedMilliseconds}ms",
                    DateTime.UtcNow,
                    requestName,
                    stopwatch.ElapsedMilliseconds);
            }
            else
            {
                stopwatch.Stop();
                _logger.LogError(
                    "{Timestamp} | {RequestName} Error handling | {ElapsedMilliseconds}ms | {ResultMessage}",
                    DateTime.UtcNow,
                    requestName,
                    stopwatch.ElapsedMilliseconds,
                    result.Message
                );
            }

            return result;
        }
    }

    [Decorator]
    public sealed class BaseCommandHandler<TBaseCommand> : ICommandHandler<TBaseCommand>
        where TBaseCommand : ICommand
    {
        private readonly ILogger<BaseCommandHandler<TBaseCommand>> _logger;
        private readonly ICommandHandler<TBaseCommand> _innerHandler;

        public BaseCommandHandler(ILogger<BaseCommandHandler<TBaseCommand>> logger, ICommandHandler<TBaseCommand> innerHandler)
        {
            _logger = logger;
            _innerHandler = innerHandler;
        }

        public async Task<Result> Handle(TBaseCommand command, CancellationToken cancellationToken = default)
        {
            var requestName = typeof(TBaseCommand).Name;

            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation(
                "{Timestamp} | Handling {RequestName}",
                DateTime.UtcNow,
                requestName);

            Result result = await _innerHandler.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                stopwatch.Stop();
                _logger.LogInformation(
                    "{Timestamp} | Handled successfully {RequestName} | {ElapsedMilliseconds}ms",
                    DateTime.UtcNow,
                    requestName,
                    stopwatch.ElapsedMilliseconds);
            }
            else
            {
                stopwatch.Stop();
                _logger.LogError(
                    "{Timestamp} | {RequestName} Error handling | {ElapsedMilliseconds}ms | {ResultMessage}",
                    DateTime.UtcNow,
                    requestName,
                    stopwatch.ElapsedMilliseconds,
                    result.Message
                );
            }

            return result;
        }
    }

    [Decorator]
    public sealed class NotificationHandler<TNotification> : INotificationHandler<TNotification>
        where TNotification : INotification
    {
        private readonly ILogger<NotificationHandler<TNotification>> _logger;
        private readonly INotificationHandler<TNotification> _innerHandler;

        public NotificationHandler(ILogger<NotificationHandler<TNotification>> logger, INotificationHandler<TNotification> innerHandler)
        {
            _logger = logger;
            _innerHandler = innerHandler;
        }

        public async Task Handle(TNotification notification, CancellationToken cancellationToken = default)
        {
            var requestName = typeof(TNotification).Name;
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation(
                "{Timestamp} | Handling {RequestName}",
                DateTime.UtcNow,
                requestName);

            try
            {
                await _innerHandler.Handle(notification, cancellationToken);

                stopwatch.Stop();
                _logger.LogInformation(
                    "{Timestamp} | Handled successfully {RequestName} | {ElapsedMilliseconds}ms",
                    DateTime.UtcNow,
                    requestName,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (System.Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex,
                    "{Timestamp} | {RequestName} Error handling | {ElapsedMilliseconds}ms",
                    DateTime.UtcNow,
                    requestName,
                    stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}