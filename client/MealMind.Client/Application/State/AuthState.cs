using Blazored.LocalStorage;
using MealMind.Shared.Contracts.Dto.Identity;

namespace MealMind.Client.Application.State;

public class AuthState
{
    private readonly ILocalStorageService _localStorageService;
    private string? _token;
    public event Action? OnChange;
    public bool IsAuthenticated => CurrentUser != null && !string.IsNullOrEmpty(_token);
    public IdentityDto? CurrentUser { get; private set; }

    public AuthState(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public async Task InitializeAsync()
    {
        _token = await _localStorageService.GetItemAsync<string>("authToken");
        CurrentUser = await _localStorageService.GetItemAsync<IdentityDto>("currentUser");
        NotifyStateChanged();
    }

    public async Task LoginAsync(IdentityDto user, string token)
    {
        CurrentUser = user;
        _token = token;

        await _localStorageService.SetItemAsync("authToken", _token);
        await _localStorageService.SetItemAsync("currentUser", CurrentUser);

        NotifyStateChanged();
    }

    public async Task LogoutAsync()
    {
        CurrentUser = null;
        _token = null;

        await _localStorageService.RemoveItemAsync("authToken");
        await _localStorageService.RemoveItemAsync("currentUser");

        NotifyStateChanged();
    }

    public string? GetToken() => _token;

    private void NotifyStateChanged()
        => OnChange?.Invoke();
}