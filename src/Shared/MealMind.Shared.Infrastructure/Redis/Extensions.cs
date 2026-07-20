using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.System.Text.Json;

namespace MealMind.Shared.Infrastructure.Redis;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddRedis(IConfiguration configuration)
        {
            var options = new RedisOptions();
            configuration.GetSection(RedisOptions.SectionName).Bind(options);

            services.AddStackExchangeRedisExtensions<SystemTextJsonSerializer>(new RedisConfiguration()
            {
                ConnectionString = options.ConnectionString,
                AbortOnConnectFail = false,
            });

            return services;
        }
    }
}
