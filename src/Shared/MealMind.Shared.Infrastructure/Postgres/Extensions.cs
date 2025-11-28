using MealMind.Shared.Abstractions.Behaviors;
using MealMind.Shared.Abstractions.Kernel.Database;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Infrastructure.Postgres.Decorators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Shared.Infrastructure.Postgres;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddPostgres()
        {
            var options = services.GetOptions<PostgresOptions>("postgres");
            services.AddSingleton(options);
            services.AddSingleton(UnitOfWorkTypeRegistry.Instance);

            return services;
        }

        public IServiceCollection AddDecorators()
        {
            // TODO: Decorators disabled - nested generic classes don't work with TryDecorate
            // Need to refactor LoggingDecorator to be top-level classes
            services.TryDecorate(typeof(ICommandHandler<,>), typeof(TransactionalCommandHandlerDecorator<>));
            services.TryDecorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
            services.TryDecorate(typeof(ICommandHandler<>), typeof(LoggingDecorator.BaseCommandHandler<>));
            services.TryDecorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));

            return services;
        }

        public IServiceCollection AddPostgres<T>(IConfiguration configuration) where T : DbContext
        {
            var options = configuration.GetOptions<PostgresOptions>("postgres");
            services.AddDbContext<T>(x => x.UseNpgsql(options.ConnectionString).UseNpgsql(o => o.UseVector()));
            return services;
        }

        public IServiceCollection AddRedis()
        {
            // services.AddStackExchangeRedisCache(options =>
            // {
            //     var redisOptions = services.GetOptions<RedisOptions>("redis");
            //     options.Configuration = redisOptions.ConnectionString;
            // });

            return services;
        }

        public IServiceCollection AddUnitOfWork<TUnitOfWork, TImplementation>()
            where TUnitOfWork : class, IBaseUnitOfWork where TImplementation : class, TUnitOfWork
        {
            services.AddScoped<TUnitOfWork, TImplementation>();
            services.AddScoped<IBaseUnitOfWork, TImplementation>();

            UnitOfWorkTypeRegistry.Instance.Register<TUnitOfWork>();

            return services;
        }
    }
}