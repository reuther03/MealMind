using System.Reflection;
using MealMind.Shared.Abstractions.Kernel.Events;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.QueriesAndCommands.Notifications;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Shared.Infrastructure.Services;

public static class MediatrExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMediatrWithFilters(IEnumerable<Assembly> assemblies)
        {
            var handlerTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t =>
                    t is { IsAbstract: false, IsInterface: false } &&
                    t.GetInterfaces().Any(i => i.IsGenericType &&
                        (i.GetGenericTypeDefinition() == typeof(INotificationHandler<>) ||
                            i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>) ||
                            i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>) ||
                            i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                            i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))) &&
                    !t.GetCustomAttributes<DecoratorAttribute>().Any());

            foreach (var handlerType in handlerTypes)
            {
                var interfaces = handlerType.GetInterfaces()
                    .Where(i => i.IsGenericType &&
                        (i.GetGenericTypeDefinition() == typeof(INotificationHandler<>) ||
                            i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>) ||
                            i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>) ||
                            i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                            i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)));


                foreach (var handlerInterface in interfaces)
                {
                    services.AddScoped(handlerInterface, handlerType);
                }
            }

            return services;
        }
    }
}