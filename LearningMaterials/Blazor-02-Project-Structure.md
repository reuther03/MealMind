# Blazor Project Structure & Organization

## Where Should the Frontend Live?

You have several options for organizing your Blazor frontend relative to your existing MealMind backend:

### Option 1: Separate Root Folder (Recommended for MealMind)
```
MealMind/
├── src/
│   ├── Bootstrapper/
│   ├── Modules/
│   └── Shared/
├── frontend/                          ← New Blazor app
│   ├── MealMind.Frontend.csproj
│   ├── Program.cs
│   ├── Pages/
│   ├── Components/
│   └── wwwroot/
├── LearningMaterials/
└── README.md
```

**Pros**:
- ✅ Clear separation (backend vs frontend)
- ✅ Can deploy independently
- ✅ Different build/deploy pipelines
- ✅ Easier to reason about

**Cons**:
- ❌ Can't share C# code directly (need shared project)

---

### Option 2: Inside src/ (Monorepo Style)
```
MealMind/
├── src/
│   ├── Bootstrapper/
│   ├── Modules/
│   ├── Shared/
│   └── Frontend/                      ← Blazor app in src/
│       └── MealMind.Frontend/
└── README.md
```

**Pros**:
- ✅ Everything in one place
- ✅ Easier to share code between backend/frontend
- ✅ Single solution file can reference both

**Cons**:
- ❌ Blurs backend/frontend boundary

---

### Option 3: Shared Contracts (Best of Both Worlds)
```
MealMind/
├── src/
│   ├── Bootstrapper/
│   ├── Modules/
│   └── Shared/
│       └── MealMind.Shared.Contracts/  ← NEW: DTOs, Results, etc.
│           ├── Identity/
│           │   ├── LoginRequest.cs
│           │   └── UserDto.cs
│           ├── AiChat/
│           │   ├── StructuredResponse.cs
│           │   └── ConversationDto.cs
│           └── Common/
│               └── Result.cs
├── frontend/                           ← Blazor app references Contracts
│   ├── MealMind.Frontend.csproj
│   └── ...
└── README.md
```

**Pros**:
- ✅ Share DTOs, validation, Result<T> between backend/frontend
- ✅ Type-safe API calls (compile-time checks)
- ✅ Clear separation but with code sharing

**This is what we'll use for MealMind.**

---

## Professional Blazor Project Structure

Here's how to structure a real-world Blazor app (similar to Clean Architecture for backend):

```
frontend/
├── MealMind.Frontend.csproj
├── Program.cs                          // DI, services, startup config
├── wwwroot/                            // Static assets (CSS, images, etc.)
│   ├── css/
│   │   └── app.css
│   ├── js/
│   │   └── interop.js                  // JS interop if needed
│   └── index.html                      // Entry HTML
│
├── Pages/                              // Routable pages (@page directive)
│   ├── Index.razor                     // @page "/"
│   ├── Login.razor                     // @page "/login"
│   ├── Chat/
│   │   ├── ChatList.razor              // @page "/chat"
│   │   └── ChatRoom.razor              // @page "/chat/{id:guid}"
│   └── _Imports.razor                  // Shared using statements
│
├── Components/                         // Reusable components (no routes)
│   ├── Layout/
│   │   ├── MainLayout.razor            // App shell
│   │   ├── NavMenu.razor               // Navigation
│   │   └── Footer.razor
│   ├── Common/
│   │   ├── LoadingSpinner.razor
│   │   ├── ErrorMessage.razor
│   │   └── ConfirmDialog.razor
│   ├── Chat/
│   │   ├── MessageBubble.razor
│   │   ├── ChatInput.razor
│   │   └── ConversationCard.razor
│   └── Identity/
│       ├── LoginForm.razor
│       └── SignUpForm.razor
│
├── Services/                           // Business logic, API calls
│   ├── IAuthService.cs
│   ├── AuthService.cs
│   ├── IChatService.cs
│   ├── ChatService.cs
│   └── IApiClient.cs                   // Wrapper around HttpClient
│       └── ApiClient.cs
│
├── State/                              // App-wide state management
│   ├── AuthState.cs                    // Current user, token, etc.
│   └── ChatState.cs                    // Active conversation, messages
│
├── Models/                             // Frontend-specific models
│   ├── ViewModels/
│   │   ├── LoginViewModel.cs
│   │   └── ChatViewModel.cs
│   └── UIModels/
│       └── NotificationModel.cs
│
└── Extensions/                         // Helper extensions
    ├── HttpClientExtensions.cs
    └── ResultExtensions.cs
```

---

## Detailed Breakdown

### 1. `Program.cs` (Entry Point)

This is where you configure DI, services, and app setup (like `Program.cs` in your backend):

```csharp
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MealMind.Frontend;
using MealMind.Frontend.Services;
using MealMind.Frontend.State;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Root component
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient (points to your backend API)
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:5000") // Your MealMind API
});

// Register services (like DI in backend)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IApiClient, ApiClient>();

// Register state (scoped = per user session)
builder.Services.AddScoped<AuthState>();
builder.Services.AddScoped<ChatState>();

// Optional: Add auth
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
```

**Key points**:
- `AddScoped` = one instance per user session (like backend scoped services)
- `HttpClient` base address = your MealMind API URL
- Services registered here can be `@inject`ed in components

---

### 2. `Pages/` (Routable Components)

Pages have `@page` directive and represent routes:

**Example: `Pages/Chat/ChatList.razor`**
```razor
@page "/chat"
@inject IChatService ChatService
@inject NavigationManager Navigation

<PageTitle>My Conversations</PageTitle>

<div class="chat-list">
    <h1>Conversations</h1>

    <button @onclick="CreateNew">New Conversation</button>

    @if (isLoading)
    {
        <LoadingSpinner />
    }
    else if (conversations.Any())
    {
        @foreach (var conv in conversations)
        {
            <ConversationCard
                Conversation="@conv"
                OnClick="@(() => OpenConversation(conv.Id))"
            />
        }
    }
    else
    {
        <p>No conversations yet. Create one!</p>
    }
</div>

@code {
    private List<ConversationDto> conversations = new();
    private bool isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        var result = await ChatService.GetConversationsAsync();

        if (result.IsSuccess)
            conversations = result.Value;

        isLoading = false;
    }

    private void OpenConversation(Guid id)
    {
        Navigation.NavigateTo($"/chat/{id}");
    }

    private void CreateNew()
    {
        Navigation.NavigateTo("/chat/new");
    }
}
```

**Structure**:
- Top: Directives (`@page`, `@inject`)
- Middle: HTML/markup
- Bottom: `@code` block with logic

---

### 3. `Components/` (Reusable Pieces)

Components DON'T have `@page` - they're building blocks:

**Example: `Components/Chat/ConversationCard.razor`**
```razor
<div class="conversation-card" @onclick="HandleClick">
    <h3>@Conversation.Title</h3>
    <p class="last-message">@Conversation.LastMessage</p>
    <span class="timestamp">@Conversation.UpdatedAt.ToString("MMM dd, yyyy")</span>
</div>

@code {
    [Parameter, EditorRequired]
    public ConversationDto Conversation { get; set; } = null!;

    [Parameter]
    public EventCallback OnClick { get; set; }

    private async Task HandleClick()
    {
        await OnClick.InvokeAsync();
    }
}
```

**Key features**:
- `[Parameter]` for props
- `[EditorRequired]` for required props (compile warning if missing)
- `EventCallback` for parent communication

---

### 4. `Services/` (API Calls & Business Logic)

Services encapsulate API calls (like repositories in backend):

**Example: `Services/ChatService.cs`**
```csharp
public interface IChatService
{
    Task<Result<List<ConversationDto>>> GetConversationsAsync();
    Task<Result<ConversationDto>> CreateConversationAsync(string title);
    Task<Result<StructuredResponse>> SendMessageAsync(Guid conversationId, string prompt);
}

public class ChatService : IChatService
{
    private readonly HttpClient _http;

    public ChatService(HttpClient http)
    {
        _http = http;
    }

    public async Task<Result<List<ConversationDto>>> GetConversationsAsync()
    {
        try
        {
            var response = await _http.GetAsync("api/aichat/conversations");
            var result = await response.Content.ReadFromJsonAsync<Result<List<ConversationDto>>>();
            return result ?? Result<List<ConversationDto>>.Failure("Failed to parse response");
        }
        catch (Exception ex)
        {
            return Result<List<ConversationDto>>.Failure(ex.Message);
        }
    }

    public async Task<Result<ConversationDto>> CreateConversationAsync(string title)
    {
        var response = await _http.PostAsJsonAsync("api/aichat/conversations", new { Title = title });
        return await response.Content.ReadFromJsonAsync<Result<ConversationDto>>()
            ?? Result<ConversationDto>.Failure("Failed to create conversation");
    }

    public async Task<Result<StructuredResponse>> SendMessageAsync(Guid conversationId, string prompt)
    {
        var response = await _http.PostAsJsonAsync($"api/aichat/conversations/{conversationId}/messages",
            new { Prompt = prompt });

        return await response.Content.ReadFromJsonAsync<Result<StructuredResponse>>()
            ?? Result<StructuredResponse>.Failure("Failed to send message");
    }
}
```

**Why services?**
- ✅ Testable (can mock `IChatService`)
- ✅ Reusable across components
- ✅ Keeps components thin (UI logic only)
- ✅ Encapsulates API details

---

### 5. `State/` (App-Wide State)

For state that needs to be shared across components:

**Example: `State/AuthState.cs`**
```csharp
public class AuthState
{
    private string? _token;
    private UserDto? _currentUser;

    public event Action? OnChange;

    public bool IsAuthenticated => _token != null;
    public UserDto? CurrentUser => _currentUser;

    public void Login(string token, UserDto user)
    {
        _token = token;
        _currentUser = user;

        // Save to localStorage (optional)
        // await JSRuntime.InvokeVoidAsync("localStorage.setItem", "token", token);

        NotifyStateChanged();
    }

    public void Logout()
    {
        _token = null;
        _currentUser = null;
        NotifyStateChanged();
    }

    public string? GetToken() => _token;

    private void NotifyStateChanged() => OnChange?.Invoke();
}
```

**Usage in components**:
```razor
@inject AuthState AuthState
@implements IDisposable

<div>
    @if (AuthState.IsAuthenticated)
    {
        <p>Welcome, @AuthState.CurrentUser?.Name!</p>
        <button @onclick="Logout">Logout</button>
    }
    else
    {
        <p>Please log in</p>
    }
</div>

@code {
    protected override void OnInitialized()
    {
        AuthState.OnChange += StateHasChanged;
    }

    private void Logout()
    {
        AuthState.Logout();
    }

    public void Dispose()
    {
        AuthState.OnChange -= StateHasChanged;
    }
}
```

**Key pattern**:
- State object with `OnChange` event
- Components subscribe in `OnInitialized`
- Components unsubscribe in `Dispose`
- When state changes, all subscribed components re-render

---

### 6. `Models/` (Frontend-Specific Models)

**ViewModels** for forms:
```csharp
public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    public string Password { get; set; } = "";
}
```

**UIModels** for display logic:
```csharp
public class NotificationModel
{
    public string Message { get; set; } = "";
    public NotificationType Type { get; set; } // Success, Error, Info
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
```

---

### 7. `_Imports.razor` (Shared Using Statements)

Instead of repeating `@using` in every file:

**`Pages/_Imports.razor`**:
```razor
@using System.Net.Http
@using System.Net.Http.Json
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.AspNetCore.Components.WebAssembly.Http
@using MealMind.Frontend
@using MealMind.Frontend.Components
@using MealMind.Frontend.Components.Common
@using MealMind.Frontend.Services
@using MealMind.Frontend.State
@using MealMind.Shared.Contracts.AiChat
@using MealMind.Shared.Contracts.Identity
@using MealMind.Shared.Abstractions.Kernel.Primitives.Result
```

Now all files in `Pages/` can use these without `@using`.

---

## Layout Structure

Blazor uses layouts for consistent UI:

**`Components/Layout/MainLayout.razor`**:
```razor
@inherits LayoutComponentBase

<div class="page">
    <div class="sidebar">
        <NavMenu />
    </div>

    <main>
        <div class="top-row">
            <AuthDisplay />
        </div>

        <article class="content">
            @Body  @* Page content renders here *@
        </article>
    </main>
</div>
```

**Set layout in page**:
```razor
@page "/chat"
@layout MainLayout

<h1>Chat</h1>
```

Or set default in `App.razor`:
```razor
<Router AppAssembly="@typeof(App).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
    </Found>
    <NotFound>
        <p>Sorry, there's nothing at this address.</p>
    </NotFound>
</Router>
```

---

## File Organization Best Practices

### 1. Group by Feature (Not by Type)

❌ **Bad** (grouped by type):
```
Components/
├── ConversationCard.razor
├── MessageBubble.razor
├── ChatInput.razor
├── LoginForm.razor
└── SignUpForm.razor
```

✅ **Good** (grouped by feature):
```
Components/
├── Chat/
│   ├── ConversationCard.razor
│   ├── MessageBubble.razor
│   └── ChatInput.razor
└── Identity/
    ├── LoginForm.razor
    └── SignUpForm.razor
```

**Why**: Easier to find related components, mirrors backend module structure.

---

### 2. Colocate Styles (CSS Isolation)

Blazor supports scoped CSS:

**`ConversationCard.razor`**:
```razor
<div class="card">
    <h3>@Title</h3>
</div>
```

**`ConversationCard.razor.css`** (same folder):
```css
.card {
    border: 1px solid #ddd;
    padding: 16px;
    border-radius: 8px;
}

.card h3 {
    margin: 0;
    color: #333;
}
```

**Result**: Styles ONLY apply to this component (scoped automatically).

---

### 3. Shared Components in `Common/`

**Components used everywhere**:
```
Components/
├── Common/
│   ├── Button.razor
│   ├── LoadingSpinner.razor
│   ├── ErrorMessage.razor
│   └── Modal.razor
├── Chat/
└── Identity/
```

---

### 4. One Component Per File

❌ **Bad**:
```razor
@* UserList.razor *@

<UserCard ... />
<UserCard ... />

@code { ... }

<UserCard>
    @* UserCard defined inline *@
</UserCard>
```

✅ **Good**:
```
Components/
├── Users/
│   ├── UserList.razor
│   └── UserCard.razor
```

**Why**: Easier to navigate, reuse, and test.

---

## Typical File Sizes

For reference, typical Blazor component sizes:

- **Small component** (Button, LoadingSpinner): 20-50 lines
- **Medium component** (LoginForm, MessageBubble): 50-150 lines
- **Large component** (ChatRoom page): 150-300 lines
- **Too large** (>300 lines): Split into smaller components

**Rule of thumb**: If `@code` block is >100 lines, extract logic to a service.

---

## Folder Structure for MealMind (Recommended)

```
MealMind/
├── src/                                # Backend
│   ├── Bootstrapper/
│   ├── Modules/
│   │   ├── Identity/
│   │   ├── Nutrition/
│   │   └── AiChat/
│   └── Shared/
│       └── MealMind.Shared.Contracts/  # NEW: Shared DTOs
│           ├── Common/
│           │   └── Result.cs
│           ├── Identity/
│           │   ├── LoginRequest.cs
│           │   ├── SignUpRequest.cs
│           │   └── UserDto.cs
│           └── AiChat/
│               ├── ConversationDto.cs
│               ├── MessageDto.cs
│               └── StructuredResponse.cs
│
├── frontend/                           # Blazor frontend
│   ├── MealMind.Frontend.csproj
│   ├── Program.cs
│   ├── App.razor
│   ├── wwwroot/
│   ├── Pages/
│   │   ├── Index.razor
│   │   ├── Login.razor
│   │   ├── SignUp.razor
│   │   └── Chat/
│   │       ├── ChatList.razor
│   │       └── ChatRoom.razor
│   ├── Components/
│   │   ├── Layout/
│   │   ├── Common/
│   │   ├── Identity/
│   │   └── Chat/
│   ├── Services/
│   │   ├── AuthService.cs
│   │   └── ChatService.cs
│   └── State/
│       └── AuthState.cs
│
└── LearningMaterials/
```

---

## How Frontend Talks to Backend

### 1. Create Shared Contracts Project

```bash
dotnet new classlib -n MealMind.Shared.Contracts -o src/Shared/MealMind.Shared.Contracts
```

**Move your DTOs here**:
- `Result<T>`
- `ConversationDto`
- `StructuredResponse`
- `LoginRequest`
- etc.

### 2. Reference in Both Projects

**Backend (`MealMind.Bootstrapper.csproj`)**:
```xml
<ItemGroup>
  <ProjectReference Include="..\Shared\MealMind.Shared.Contracts\MealMind.Shared.Contracts.csproj" />
</ItemGroup>
```

**Frontend (`MealMind.Frontend.csproj`)**:
```xml
<ItemGroup>
  <ProjectReference Include="..\src\Shared\MealMind.Shared.Contracts\MealMind.Shared.Contracts.csproj" />
</ItemGroup>
```

### 3. Use in Frontend

```csharp
// Both projects use the SAME Result<T> class
var result = await Http.GetFromJsonAsync<Result<ConversationDto>>("api/chat/conversations");

if (result.IsSuccess)
{
    // result.Value is ConversationDto (same as backend)
}
```

**Benefit**: Change DTO in one place, both backend/frontend get updated.

---

## Next Steps

Now that you understand project structure:
1. **Read**: Blazor-03-Component-Patterns.md - Learn advanced component patterns
2. **Read**: Blazor-04-Clean-Architecture.md - How to apply Clean Architecture to Blazor

---

## Key Takeaways

✅ Frontend lives in separate `/frontend` folder
✅ Share DTOs via `MealMind.Shared.Contracts` project
✅ Structure: Pages (routable) + Components (reusable) + Services (logic)
✅ Use DI in `Program.cs` just like backend
✅ State management via scoped services with events
✅ Group files by feature, not type
✅ Colocate CSS with components
✅ Keep components small (<300 lines)

**Bottom line**: Blazor project structure mirrors backend Clean Architecture - you already know this pattern!
