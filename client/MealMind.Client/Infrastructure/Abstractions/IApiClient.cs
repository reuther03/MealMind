using MealMind.Shared.Contracts.Result;

namespace MealMind.Client.Infrastructure.Abstractions;

public interface IApiClient
{
    const string AiChatModulePrefix = "/aiChat-module";
    const string NutritionModulePrefix = "/nutrition-module";
    const string IdentityModulePrefix = "/identity-module";

    Task<Result<T>> GetAsync<T>(string endpoint);
    Task<Result<T>> PostAsync<T>(string endpoint, object? data = null);
    Task<Result<T>> PutAsync<T>(string endpoint, object data);
    Task<Result> DeleteAsync(string endpoint);
}