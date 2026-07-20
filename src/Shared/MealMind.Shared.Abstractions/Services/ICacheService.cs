namespace MealMind.Shared.Abstractions.Services;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

    Task GetOrSetAsync<T>(string key, Func<Task<T>> valueFactory, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
}
