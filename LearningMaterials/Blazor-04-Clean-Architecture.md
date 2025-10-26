# Blazor + Clean Architecture Integration

## Mapping Backend Concepts to Frontend

Your MealMind backend uses Clean Architecture with modular monolith. Let's map those concepts to Blazor:

| Backend Concept | Frontend Equivalent | Purpose |
|----------------|-------------------|---------|
| **Module** | Feature Folder | Group related pages/components |
| **Domain Entity** | DTO (from Shared.Contracts) | Data structures |
| **Repository** | Service (API client) | Data access |
| **Command/Query** | Service Method | Operations |
| **Command Handler** | Component `@code` block | Business logic |
| **Result<T>** | Same `Result<T>` | Error handling |
| **Integration Event** | SignalR (optional) | Real-time updates |
| **DI Container** | `Program.cs` services | Dependency injection |

**Key insight**: Frontend mirrors backend architecture, just UI-focused.

---

## Clean Architecture in Blazor

While backend has layers (Domain, Application, Infrastructure), frontend is simpler:

```
Frontend Architecture (3 Layers):
┌─────────────────────────────────────┐
│  Presentation Layer                  │  ← Pages + Components
│  (UI, user interaction)              │
└─────────────────────────────────────┘
           ↓ uses
┌─────────────────────────────────────┐
│  Application Layer                   │  ← Services + State
│  (business logic, API calls)         │
└─────────────────────────────────────┘
           ↓ uses
┌─────────────────────────────────────┐
│  Contracts Layer                     │  ← Shared DTOs, Result<T>
│  (shared with backend)               │
└─────────────────────────────────────┘
```

**Why simpler?**
- No database (backend handles that)
- No domain logic (backend handles that)
- Frontend focuses on UI + API calls

---

## Folder Structure (Clean Architecture Style)

```
frontend/
├── MealMind.Frontend.csproj
├── Program.cs                          // DI, app configuration
│
├── Presentation/                       // UI Layer
│   ├── Pages/                          // Routable pages
│   │   ├── Index.razor
│   │   ├── Identity/
│   │   │   ├── Login.razor
│   │   │   └── SignUp.razor
│   │   ├── Chat/
│   │   │   ├── ChatList.razor
│   │   │   └── ChatRoom.razor
│   │   └── Nutrition/
│   │       └── Profile.razor
│   │
│   ├── Components/                     // Reusable UI components
│   │   ├── Layout/
│   │   │   ├── MainLayout.razor
│   │   │   └── NavMenu.razor
│   │   ├── Common/
│   │   │   ├── LoadingSpinner.razor
│   │   │   └── ErrorMessage.razor
│   │   ├── Identity/
│   │   │   └── LoginForm.razor
│   │   ├── Chat/
│   │   │   ├── MessageBubble.razor
│   │   │   └── ConversationCard.razor
│   │   └── Nutrition/
│   │       └── NutritionCard.razor
│   │
│   └── wwwroot/                        // Static assets
│       ├── css/
│       └── js/
│
├── Application/                        // Business Logic Layer
│   ├── Services/                       // API communication
│   │   ├── Identity/
│   │   │   ├── IAuthService.cs
│   │   │   └── AuthService.cs
│   │   ├── Chat/
│   │   │   ├── IChatService.cs
│   │   │   └── ChatService.cs
│   │   └── Nutrition/
│   │       ├── INutritionService.cs
│   │       └── NutritionService.cs
│   │
│   ├── State/                          // Global state management
│   │   ├── AuthState.cs
│   │   ├── ChatState.cs
│   │   └── NotificationState.cs
│   │
│   ├── Models/                         // Frontend-specific models
│   │   ├── ViewModels/
│   │   │   ├── LoginViewModel.cs
│   │   │   └── ChatViewModel.cs
│   │   └── UIModels/
│   │       └── NotificationModel.cs
│   │
│   └── Infrastructure/                 // Cross-cutting concerns
│       ├── HttpClients/
│       │   └── ApiClient.cs
│       └── Extensions/
│           ├── ResultExtensions.cs
│           └── HttpClientExtensions.cs
│
└── (Reference to MealMind.Shared.Contracts)  // Shared DTOs
```

**Benefits**:
- ✅ Clear separation of concerns
- ✅ Mirrors backend module structure
- ✅ Easy to navigate (Identity, Chat, Nutrition)
- ✅ Testable (services can be mocked)

---

## Layer Responsibilities

### Presentation Layer (`Presentation/`)

**Responsibilities**:
- Render UI
- Handle user input
- Route navigation
- Display data from services

**Rules**:
- ❌ No direct HttpClient calls
- ❌ No business logic
- ✅ Call services
- ✅ Manage local UI state (loading, errors)

**Example**: `Pages/Chat/ChatRoom.razor`
```razor
@page "/chat/{ConversationId:guid}"
@inject IChatService ChatService
@inject NavigationManager Navigation

<div class="chat-room">
    @if (isLoading)
    {
        <LoadingSpinner />
    }
    else if (error != null)
    {
        <ErrorMessage Message="@error" />
    }
    else
    {
        <MessageList Messages="@messages" />
        <ChatInput OnSend="HandleSendMessage" />
    }
</div>

@code {
    [Parameter]
    public Guid ConversationId { get; set; }

    private List<MessageDto> messages = new();
    private bool isLoading = true;
    private string? error;

    protected override async Task OnInitializedAsync()
    {
        var result = await ChatService.GetMessagesAsync(ConversationId);

        if (result.IsSuccess)
            messages = result.Value;
        else
            error = result.Error;

        isLoading = false;
    }

    private async Task HandleSendMessage(string prompt)
    {
        var result = await ChatService.SendMessageAsync(ConversationId, prompt);

        if (result.IsSuccess)
            messages.Add(result.Value);
        else
            error = result.Error;
    }
}
```

**Notice**:
- ✅ Component is thin (just coordinates)
- ✅ All logic in `ChatService`
- ✅ Uses `Result<T>` from backend

---

### Application Layer (`Application/`)

**Responsibilities**:
- API communication
- Error handling
- State management
- Caching (if needed)

**Rules**:
- ❌ No UI logic (Razor syntax)
- ✅ Return `Result<T>`
- ✅ Handle HTTP errors
- ✅ Transform DTOs if needed

**Example**: `Application/Services/Chat/ChatService.cs`
```csharp
public interface IChatService
{
    Task<Result<List<ConversationDto>>> GetConversationsAsync();
    Task<Result<List<MessageDto>>> GetMessagesAsync(Guid conversationId);
    Task<Result<MessageDto>> SendMessageAsync(Guid conversationId, string prompt);
    Task<Result<ConversationDto>> CreateConversationAsync(string title);
}

public class ChatService : IChatService
{
    private readonly HttpClient _http;
    private readonly AuthState _authState;

    public ChatService(HttpClient http, AuthState authState)
    {
        _http = http;
        _authState = authState;
    }

    public async Task<Result<List<ConversationDto>>> GetConversationsAsync()
    {
        try
        {
            AddAuthHeader();

            var response = await _http.GetAsync("api/aichat/conversations");

            if (!response.IsSuccessStatusCode)
                return Result<List<ConversationDto>>.Failure($"API error: {response.StatusCode}");

            var result = await response.Content.ReadFromJsonAsync<Result<List<ConversationDto>>>();

            return result ?? Result<List<ConversationDto>>.Failure("Failed to parse response");
        }
        catch (Exception ex)
        {
            return Result<List<ConversationDto>>.Failure($"Network error: {ex.Message}");
        }
    }

    public async Task<Result<MessageDto>> SendMessageAsync(Guid conversationId, string prompt)
    {
        try
        {
            AddAuthHeader();

            var command = new GetChatResponseCommand(conversationId, prompt);
            var response = await _http.PostAsJsonAsync("api/aichat/response", command);

            if (!response.IsSuccessStatusCode)
                return Result<MessageDto>.Failure($"API error: {response.StatusCode}");

            var result = await response.Content.ReadFromJsonAsync<Result<StructuredResponse>>();

            if (result?.IsSuccess == true)
            {
                // Transform StructuredResponse to MessageDto for UI
                var message = new MessageDto
                {
                    Id = Guid.NewGuid(),
                    ConversationId = conversationId,
                    Role = "Assistant",
                    Content = result.Value.Title, // Simplified
                    CreatedAt = DateTime.UtcNow
                };

                return Result<MessageDto>.Ok(message);
            }

            return Result<MessageDto>.Failure(result?.Error ?? "Unknown error");
        }
        catch (Exception ex)
        {
            return Result<MessageDto>.Failure($"Network error: {ex.Message}");
        }
    }

    private void AddAuthHeader()
    {
        var token = _authState.GetToken();
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}
```

**Benefits**:
- ✅ All API logic in one place
- ✅ Reusable across components
- ✅ Testable (can mock `HttpClient`)
- ✅ Consistent error handling

---

### Contracts Layer (`MealMind.Shared.Contracts`)

**Shared between backend and frontend**:

```
src/Shared/MealMind.Shared.Contracts/
├── Common/
│   ├── Result.cs                       // Shared Result<T>
│   └── PagedResult.cs
├── Identity/
│   ├── Requests/
│   │   ├── LoginRequest.cs
│   │   └── SignUpRequest.cs
│   └── DTOs/
│       └── UserDto.cs
├── AiChat/
│   ├── Requests/
│   │   └── GetChatResponseCommand.cs
│   └── DTOs/
│       ├── ConversationDto.cs
│       ├── MessageDto.cs
│       └── StructuredResponse.cs
└── Nutrition/
    └── DTOs/
        └── NutritionProfileDto.cs
```

**Example**: `Result.cs` (used by both backend and frontend)
```csharp
namespace MealMind.Shared.Contracts.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }

    private Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Ok(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);
}
```

**Both projects reference this**, ensuring type consistency.

---

## Dependency Injection Setup

**`Program.cs`** mirrors backend DI registration:

```csharp
using MealMind.Frontend;
using MealMind.Frontend.Application.Services.Identity;
using MealMind.Frontend.Application.Services.Chat;
using MealMind.Frontend.Application.State;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Root components
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient configuration
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:5000")
});

// State management (like singletons, but scoped to user session)
builder.Services.AddScoped<AuthState>();
builder.Services.AddScoped<ChatState>();
builder.Services.AddScoped<NotificationState>();

// Services (like repositories in backend)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<INutritionService, NutritionService>();

// Infrastructure services
builder.Services.AddScoped<IApiClient, ApiClient>();

// Optional: Add authorization
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
```

**Compare to backend**:
- Backend `Program.cs`: Registers modules, repositories, MediatR
- Frontend `Program.cs`: Registers services, state, HttpClient

**Same pattern, different purpose.**

---

## State Management (Like Domain Events)

In backend, you have integration events. In frontend, you have state objects:

**`Application/State/AuthState.cs`**:
```csharp
namespace MealMind.Frontend.Application.State;

public class AuthState
{
    private string? _token;
    private UserDto? _currentUser;

    // Event (like integration event in backend)
    public event Action? OnAuthChanged;

    public bool IsAuthenticated => _token != null;
    public UserDto? CurrentUser => _currentUser;

    public void Login(string token, UserDto user)
    {
        _token = token;
        _currentUser = user;

        // Persist to localStorage (browser storage)
        // (Implementation omitted for brevity)

        NotifyAuthChanged();
    }

    public void Logout()
    {
        _token = null;
        _currentUser = null;
        NotifyAuthChanged();
    }

    public string? GetToken() => _token;

    private void NotifyAuthChanged()
    {
        OnAuthChanged?.Invoke();
    }
}
```

**Components subscribe**:
```razor
@inject AuthState AuthState
@implements IDisposable

@code {
    protected override void OnInitialized()
    {
        AuthState.OnAuthChanged += HandleAuthChanged;
    }

    private void HandleAuthChanged()
    {
        StateHasChanged(); // Re-render when auth changes
    }

    public void Dispose()
    {
        AuthState.OnAuthChanged -= HandleAuthChanged;
    }
}
```

**Compare to backend**:
- Backend: `await _publisher.Publish(new IdentityUserCreatedEvent(...))`
- Frontend: `AuthState.OnAuthChanged?.Invoke()`

**Same pub/sub pattern.**

---

## API Client Pattern (Like Repository Pattern)

Create a base API client (like `Repository<T>` in backend):

**`Application/Infrastructure/HttpClients/ApiClient.cs`**:
```csharp
public interface IApiClient
{
    Task<Result<T>> GetAsync<T>(string endpoint);
    Task<Result<T>> PostAsync<T>(string endpoint, object? data = null);
    Task<Result<T>> PutAsync<T>(string endpoint, object data);
    Task<Result> DeleteAsync(string endpoint);
}

public class ApiClient : IApiClient
{
    private readonly HttpClient _http;
    private readonly AuthState _authState;

    public ApiClient(HttpClient http, AuthState authState)
    {
        _http = http;
        _authState = authState;
    }

    public async Task<Result<T>> GetAsync<T>(string endpoint)
    {
        try
        {
            AddAuthHeader();
            var response = await _http.GetAsync(endpoint);
            return await ParseResponse<T>(response);
        }
        catch (Exception ex)
        {
            return Result<T>.Failure($"Network error: {ex.Message}");
        }
    }

    public async Task<Result<T>> PostAsync<T>(string endpoint, object? data = null)
    {
        try
        {
            AddAuthHeader();
            var response = await _http.PostAsJsonAsync(endpoint, data);
            return await ParseResponse<T>(response);
        }
        catch (Exception ex)
        {
            return Result<T>.Failure($"Network error: {ex.Message}");
        }
    }

    private async Task<Result<T>> ParseResponse<T>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return Result<T>.Failure($"API error ({response.StatusCode}): {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<Result<T>>();
        return result ?? Result<T>.Failure("Failed to parse response");
    }

    private void AddAuthHeader()
    {
        var token = _authState.GetToken();
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}
```

**Services use it**:
```csharp
public class ChatService : IChatService
{
    private readonly IApiClient _apiClient;

    public ChatService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<Result<List<ConversationDto>>> GetConversationsAsync()
    {
        return await _apiClient.GetAsync<List<ConversationDto>>("api/aichat/conversations");
    }

    public async Task<Result<MessageDto>> SendMessageAsync(Guid conversationId, string prompt)
    {
        return await _apiClient.PostAsync<MessageDto>(
            "api/aichat/response",
            new { ConversationId = conversationId, Prompt = prompt }
        );
    }
}
```

**Benefits**:
- ✅ DRY (no repeated try-catch in every service)
- ✅ Consistent error handling
- ✅ Auth header added automatically
- ✅ Easy to add logging, caching, retry logic

---

## Feature Modules (Like Backend Modules)

Organize by feature, mirroring backend modules:

### Identity Module (Frontend)

```
Presentation/
└── Pages/Identity/
    ├── Login.razor
    └── SignUp.razor
└── Components/Identity/
    ├── LoginForm.razor
    └── SignUpForm.razor

Application/
└── Services/Identity/
    ├── IAuthService.cs
    └── AuthService.cs
└── State/
    └── AuthState.cs
```

### AiChat Module (Frontend)

```
Presentation/
└── Pages/Chat/
    ├── ChatList.razor
    └── ChatRoom.razor
└── Components/Chat/
    ├── MessageBubble.razor
    ├── ConversationCard.razor
    └── ChatInput.razor

Application/
└── Services/Chat/
    ├── IChatService.cs
    └── ChatService.cs
└── State/
    └── ChatState.cs
```

**Notice**: Same module structure as backend!

---

## Validation (Shared with Backend)

Use **DataAnnotations** (same as backend):

**Backend**: `SignUpCommand.cs`
```csharp
public record SignUpCommand(
    [Required, MinLength(3)] string Username,
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string InputPassword
) : ICommand<Guid>;
```

**Frontend**: `SignUpViewModel.cs`
```csharp
public class SignUpViewModel
{
    [Required(ErrorMessage = "Username is required")]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
    public string Username { get; set; } = "";

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    public string Password { get; set; } = "";
}
```

**Same validation rules!**

**Component**:
```razor
<EditForm Model="@model" OnValidSubmit="HandleSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <InputText @bind-Value="model.Username" />
    <ValidationMessage For="@(() => model.Username)" />

    <InputText @bind-Value="model.Email" />
    <ValidationMessage For="@(() => model.Email)" />

    <InputText @bind-Value="model.Password" type="password" />
    <ValidationMessage For="@(() => model.Password)" />

    <button type="submit">Sign Up</button>
</EditForm>

@code {
    private SignUpViewModel model = new();

    private async Task HandleSubmit()
    {
        // Model is already validated by EditForm
        var result = await AuthService.SignUpAsync(
            model.Username,
            model.Email,
            model.Password
        );

        if (result.IsSuccess)
            Navigation.NavigateTo("/");
        else
            errorMessage = result.Error;
    }
}
```

**Benefits**:
- ✅ Client-side validation (instant feedback)
- ✅ Server-side validation (security)
- ✅ Same rules, no duplication

---

## Error Handling (Consistent with Backend)

**Backend** returns `Result<T>`:
```csharp
return Result<ConversationDto>.Failure("Conversation not found");
```

**Frontend** handles it:
```csharp
var result = await ChatService.GetConversationAsync(id);

if (result.IsSuccess)
{
    conversation = result.Value;
}
else
{
    errorMessage = result.Error; // "Conversation not found"
    ShowErrorNotification(result.Error);
}
```

**Same pattern throughout the stack.**

---

## Configuration (Like appsettings.json)

**`wwwroot/appsettings.json`** (frontend config):
```json
{
  "ApiBaseUrl": "https://localhost:5000",
  "AppName": "MealMind",
  "Features": {
    "EnableNotifications": true,
    "MaxConversations": 10
  }
}
```

**Access in code**:
```csharp
// Program.cs
var apiBaseUrl = builder.Configuration["ApiBaseUrl"];

// In components
@inject IConfiguration Configuration

@code {
    private string appName = Configuration["AppName"] ?? "MealMind";
}
```

**For environment-specific configs**:
- `appsettings.json` - default
- `appsettings.Development.json` - dev overrides
- `appsettings.Production.json` - prod overrides

---

## Testing Strategy

### Unit Tests (Services)

```csharp
public class ChatServiceTests
{
    [Fact]
    public async Task GetConversations_ReturnsSuccess_WhenApiSucceeds()
    {
        // Arrange
        var mockHttp = new Mock<HttpClient>();
        var mockAuthState = new Mock<AuthState>();
        var service = new ChatService(mockHttp.Object, mockAuthState.Object);

        // Setup mock to return success
        // ...

        // Act
        var result = await service.GetConversationsAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(5);
    }
}
```

### Component Tests (bUnit)

```csharp
public class LoginPageTests
{
    [Fact]
    public void LoginPage_ShowsError_WhenInvalidCredentials()
    {
        // Arrange
        using var ctx = new TestContext();
        var mockAuthService = ctx.Services.AddMockAuthService();

        // Act
        var cut = ctx.RenderComponent<Login>();
        cut.Find("#email").Change("invalid@email.com");
        cut.Find("#password").Change("wrongpassword");
        cut.Find("button[type=submit]").Click();

        // Assert
        cut.Find(".error-message").TextContent.Should().Contain("Invalid credentials");
    }
}
```

---

## CORS Configuration (Backend)

Don't forget to enable CORS in your backend for frontend to call API:

**Backend `Program.cs`**:
```csharp
// Add this to your MealMind backend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:5001") // Blazor dev URL
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// ...

app.UseCors("AllowFrontend");
```

---

## Project References Summary

```
MealMind.Frontend.csproj
│
├─ References:
│  └─ MealMind.Shared.Contracts.csproj  // Shared DTOs, Result<T>
│
├─ NuGet Packages:
│  ├─ Microsoft.AspNetCore.Components.WebAssembly
│  ├─ Microsoft.AspNetCore.Components.WebAssembly.DevServer
│  └─ System.Net.Http.Json
```

---

## Next Steps

You now understand how Clean Architecture applies to Blazor:

1. **Practice**: Create a simple service (e.g., `WeatherService`)
2. **Build**: Implement Login page using patterns from this guide
3. **Integrate**: Connect to your MealMind backend
4. **Iterate**: Add Chat, Nutrition modules following same structure

---

## Key Takeaways

✅ Frontend mirrors backend module structure (Identity, Chat, Nutrition)
✅ Services = Repositories (handle API calls)
✅ State = Integration Events (pub/sub pattern)
✅ Result<T> shared between backend/frontend
✅ DataAnnotations validation shared
✅ DI works the same way
✅ Clear layer separation (Presentation, Application, Contracts)
✅ Testable architecture
✅ Consistent error handling
✅ Feature-based organization

**Bottom line**: Clean Architecture isn't just for backend - apply the same principles to frontend for consistency, maintainability, and team alignment.

---

## Full Example: Login Flow (Backend to Frontend)

### 1. Shared Contract

`MealMind.Shared.Contracts/Identity/LoginRequest.cs`:
```csharp
public record LoginRequest(string Email, string Password);
```

`MealMind.Shared.Contracts/Identity/LoginResponse.cs`:
```csharp
public record LoginResponse(string Token, UserDto User);
```

### 2. Backend Endpoint

`MealMind.Modules.Identity.Api/LoginEndpoint.cs`:
```csharp
public class LoginEndpoint : EndpointBase
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("login", async (LoginRequest request, ISender sender) =>
        {
            var command = new LoginCommand(request.Email, request.Password);
            var result = await sender.Send(command);

            return result.IsSuccess
                ? Results.Ok(result)
                : Results.BadRequest(result);
        });
    }
}
```

### 3. Frontend Service

`Application/Services/Identity/AuthService.cs`:
```csharp
public async Task<Result<LoginResponse>> LoginAsync(string email, string password)
{
    var response = await _http.PostAsJsonAsync("api/identity/login",
        new LoginRequest(email, password));

    var result = await response.Content.ReadFromJsonAsync<Result<LoginResponse>>();

    if (result?.IsSuccess == true)
    {
        _authState.Login(result.Value.Token, result.Value.User);
    }

    return result ?? Result<LoginResponse>.Failure("Failed to login");
}
```

### 4. Frontend Page

`Presentation/Pages/Identity/Login.razor`:
```razor
@page "/login"
@inject IAuthService AuthService
@inject NavigationManager Navigation

<h1>Login</h1>

<EditForm Model="@model" OnValidSubmit="HandleLogin">
    <DataAnnotationsValidator />

    <InputText @bind-Value="model.Email" />
    <ValidationMessage For="@(() => model.Email)" />

    <InputText @bind-Value="model.Password" type="password" />
    <ValidationMessage For="@(() => model.Password)" />

    @if (errorMessage != null)
    {
        <div class="error">@errorMessage</div>
    }

    <button type="submit" disabled="@isLoading">
        @(isLoading ? "Logging in..." : "Login")
    </button>
</EditForm>

@code {
    private LoginViewModel model = new();
    private bool isLoading;
    private string? errorMessage;

    private async Task HandleLogin()
    {
        isLoading = true;
        errorMessage = null;

        var result = await AuthService.LoginAsync(model.Email, model.Password);

        if (result.IsSuccess)
        {
            Navigation.NavigateTo("/");
        }
        else
        {
            errorMessage = result.Error;
        }

        isLoading = false;
    }
}
```

**Notice how data flows**:
1. User enters credentials → ViewModel
2. ViewModel → AuthService
3. AuthService → Backend API (via HttpClient)
4. Backend → Result<LoginResponse>
5. Frontend Service → Updates AuthState
6. Page → Redirects to home

**Clean, predictable, type-safe flow.**
