using MealMind.Shared.Abstractions.Services;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace MealMind.Shared.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IRedisDatabase _redisDatabase;

    public CacheService(IRedisDatabase redisDatabase)
    {
        _redisDatabase = redisDatabase;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        return await _redisDatabase.Database.StringGetAsync(key) is var value && value.HasValue
            ? System.Text.Json.JsonSerializer.Deserialize<T>(value.ToString())
            : default;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var valueAsString = System.Text.Json.JsonSerializer.Serialize(value);
        await _redisDatabase.Database.StringSetAsync(key, valueAsString,
            expiration ?? new Expiration(TimeSpan.FromMinutes(0)));
    }

    public Task GetOrSetAsync<T>(string key, Func<Task<T>> valueFactory, TimeSpan? expiration = null)
    {
        return GetAsync<T>(key).ContinueWith(async task =>
        {
            if (task.Result != null)
            {
                return task.Result;
            }

            var value = await valueFactory();
            await SetAsync(key, value, expiration);
            return value;
        }).Unwrap();
    }

    public Task RemoveAsync(string key)
        => _redisDatabase.Database.KeyDeleteAsync(key);
}
