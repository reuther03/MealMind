using System.Net.Http.Headers;
using System.Net.Http.Json;
using MealMind.Client.Application.State;
using MealMind.Client.Infrastructure.Abstractions;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Client.Infrastructure;

public class ApiClient : IApiClient
{
    private readonly HttpClient _client;
    private readonly AuthState _authState;

    public ApiClient(HttpClient client, AuthState authState)
    {
        _client = client;
        _authState = authState;
    }

    public async Task<Result<T>> GetAsync<T>(string endpoint)
    {
        AddAuthHeader();
        return await ExecuteAsync<T>(() => _client.GetAsync(endpoint));
    }

    public async Task<Result<T>> PostAsync<T>(string endpoint, object? data = null)
    {
        AddAuthHeader();
        return await ExecuteAsync<T>(() => _client.PostAsJsonAsync(endpoint, data));
    }

    public async Task<Result<T>> PutAsync<T>(string endpoint, object data)
    {
        AddAuthHeader();
        return await ExecuteAsync<T>(() => _client.PutAsJsonAsync(endpoint, data));
    }

    public async Task<Result> DeleteAsync(string endpoint)
    {
        AddAuthHeader();
        return await ExecuteAsync<object?>(() => _client.DeleteAsync(endpoint));
    }

    private async Task<Result<T>> ExecuteAsync<T>(Func<Task<HttpResponseMessage>> httpCall)
    {
        try
        {
            AddAuthHeader(); // Auto-add token if logged in

            var response = await httpCall();

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result<T>.BadRequest($"API error ({response.StatusCode}): {errorContent}");
            }

            var result = await response.Content.ReadFromJsonAsync<Result<T>>();
            return result ?? Result<T>.BadRequest("Invalid response format");
        }
        catch (HttpRequestException ex)
        {
            return Result<T>.BadRequest($"Network error: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            return Result<T>.BadRequest("Request timed out");
        }
        catch (Exception ex)
        {
            return Result<T>.BadRequest($"Unexpected error: {ex.Message}");
        }
    }

    private void AddAuthHeader()
    {
        var token = _authState.GetToken();
        if (!string.IsNullOrEmpty(token))
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }
}