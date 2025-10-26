# Blazor Fundamentals: From Zero to Understanding

## What is Blazor?

Blazor is a framework for building interactive web UIs using C# instead of JavaScript. It runs your C# code either:
- **In the browser** (WebAssembly) - C# compiled to WASM, runs client-side
- **On the server** (Blazor Server) - C# runs on server, UI updates via SignalR
- **Hybrid** (Blazor United in .NET 8+) - Mix of both

**For MealMind, we'll use Blazor WebAssembly (WASM)** - it's a true SPA (Single Page Application) like React.

---

## Why Blazor Feels Different (If You Know React/Vue)

| Concept | React/Vue | Blazor |
|---------|-----------|--------|
| **Language** | JavaScript/TypeScript | C# |
| **Components** | JSX/Template syntax | Razor syntax (.razor files) |
| **State** | `useState`, `reactive` | Simple C# fields + `StateHasChanged()` |
| **Lifecycle** | `useEffect`, `mounted` | `OnInitialized`, `OnParametersSet` |
| **Events** | `onClick={handler}` | `@onclick="Handler"` |
| **Data Binding** | Controlled components | `@bind` directive |
| **Props** | Function parameters | `[Parameter]` attribute |
| **Re-rendering** | Automatic (dependency tracking) | Explicit (`StateHasChanged()`) |

**Key insight**: Blazor is MORE explicit than React. When something doesn't update, you know exactly why.

---

## Core Concepts

### 1. Components (.razor files)

A Blazor component is a `.razor` file with two parts:

```razor
@* HTML/Razor markup (top section) *@
<div class="user-card">
    <h3>@Username</h3>
    <p>@Email</p>
    <button @onclick="HandleClick">Click Me</button>
</div>

@code {
    // C# code (bottom section)
    private string Username { get; set; } = "John Doe";
    private string Email { get; set; } = "john@example.com";

    private void HandleClick()
    {
        Console.WriteLine("Button clicked!");
    }
}
```

**Compare to React**:
```tsx
// React equivalent
function UserCard() {
  const [username] = useState("John Doe");
  const [email] = useState("john@example.com");

  const handleClick = () => {
    console.log("Button clicked!");
  };

  return (
    <div className="user-card">
      <h3>{username}</h3>
      <p>{email}</p>
      <button onClick={handleClick}>Click Me</button>
    </div>
  );
}
```

**Key differences**:
- `@` instead of `{}` for C# expressions
- `@onclick` instead of `onClick` (lowercase 'o')
- Properties instead of `useState`
- No JSX, just HTML with Razor directives

---

### 2. Data Binding

**One-way binding** (display value):
```razor
<input value="@username" />
```

**Two-way binding** (sync with property):
```razor
<input @bind="username" />

@code {
    private string username = "";
}
```

When user types, `username` automatically updates. No `onChange` handlers needed.

**Compare to React**:
```tsx
// React needs explicit onChange
const [username, setUsername] = useState("");
<input
  value={username}
  onChange={(e) => setUsername(e.target.value)}
/>
```

---

### 3. Parameters (Props)

Pass data to child components:

**Parent**:
```razor
<UserCard Username="@currentUser" OnClick="HandleUserClick" />

@code {
    private string currentUser = "Alice";

    private void HandleUserClick()
    {
        Console.WriteLine("User clicked!");
    }
}
```

**Child (UserCard.razor)**:
```razor
<div @onclick="OnClick">
    <p>@Username</p>
</div>

@code {
    [Parameter]
    public string Username { get; set; } = "";

    [Parameter]
    public EventCallback OnClick { get; set; }
}
```

**Key points**:
- `[Parameter]` attribute marks properties as "props"
- `EventCallback` is like React's function props
- Type-safe at compile time (unlike JS props)

---

### 4. Component Lifecycle

Blazor has a clear, predictable lifecycle:

```razor
@code {
    // 1. Constructor runs first (rarely used)
    public MyComponent()
    {
        Console.WriteLine("Constructor");
    }

    // 2. Parameters are set
    protected override void OnInitialized()
    {
        Console.WriteLine("Component initialized - runs ONCE");
        // Perfect for: Loading initial data
    }

    protected override async Task OnInitializedAsync()
    {
        Console.WriteLine("Async initialization");
        await LoadDataAsync();
    }

    // 3. Parameters changed (runs on every re-render with new params)
    protected override void OnParametersSet()
    {
        Console.WriteLine($"Parameters updated: {Username}");
    }

    protected override async Task OnParametersSetAsync()
    {
        await RefreshDataAsync();
    }

    // 4. After component renders to DOM
    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            Console.WriteLine("First render complete");
            // Perfect for: JS interop, focus elements
        }
    }

    // 5. Component removed from DOM
    public void Dispose()
    {
        Console.WriteLine("Component disposed");
        // Perfect for: Cleanup, unsubscribe events
    }
}
```

**Lifecycle flow**:
```
Constructor → OnInitialized → OnParametersSet → Render → OnAfterRender
                ↓                    ↓
          OnInitializedAsync   OnParametersSetAsync
```

**Compare to React**:
- `OnInitialized` = `useEffect(() => {}, [])`
- `OnParametersSet` = `useEffect(() => {}, [props])`
- `OnAfterRender` = `useEffect` without deps (runs after every render)
- `Dispose` = `useEffect` cleanup function

**Why this is better**: No dependency arrays, no closure issues, no guessing when it runs.

---

### 5. State Management & Re-rendering

**Blazor does NOT automatically re-render.** You control when:

```razor
<p>Count: @count</p>
<button @onclick="Increment">Increment</button>

@code {
    private int count = 0;

    private void Increment()
    {
        count++;
        StateHasChanged(); // ← Tells Blazor to re-render
    }
}
```

**When do you need `StateHasChanged()`?**

✅ **Need it**:
- After async operations complete
- After timer callbacks
- After event handlers from non-Blazor code

❌ **Don't need it**:
- After Blazor event handlers (`@onclick`, `@onchange`, etc.)
- After parameters change
- After `OnInitialized` / `OnParametersSet`

**Example (async scenario)**:
```razor
@code {
    private List<string> messages = new();

    protected override async Task OnInitializedAsync()
    {
        await Task.Delay(2000);
        messages.Add("Loaded!");
        // NO StateHasChanged() needed - OnInitializedAsync handles it
    }

    private async Task LoadMore()
    {
        await Task.Delay(1000);
        messages.Add("More data");
        StateHasChanged(); // ← NEED this for manual async calls
    }
}
```

**Compare to React**:
- React re-renders automatically (sometimes too much)
- Blazor re-renders explicitly (you control it)

**Mental model**: If you change state and UI doesn't update → you forgot `StateHasChanged()`.

---

### 6. Dependency Injection

Blazor uses the SAME DI system as ASP.NET Core:

**Register services (Program.cs)**:
```csharp
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IConfigService, ConfigService>();
```

**Use in components**:
```razor
@inject IUserService UserService
@inject NavigationManager Navigation

<button @onclick="LoadUsers">Load</button>

@code {
    private async Task LoadUsers()
    {
        var users = await UserService.GetAllAsync();
        // ...
    }
}
```

**Lifetimes**:
- `AddSingleton` - One instance for entire app (use for config, caches)
- `AddScoped` - One instance per user session (use for services, state)
- `AddTransient` - New instance every time (rarely used)

**Compare to React**:
- React uses Context API (manual, verbose)
- Blazor uses DI (automatic, type-safe)

---

### 7. Routing

**Define routes with `@page` directive**:

```razor
@page "/chat"
@page "/chat/{ConversationId:guid}"

<h1>Chat Page</h1>
<p>Conversation ID: @ConversationId</p>

@code {
    [Parameter]
    public Guid ConversationId { get; set; }
}
```

**Navigate programmatically**:
```razor
@inject NavigationManager Navigation

<button @onclick="GoToChat">Open Chat</button>

@code {
    private void GoToChat()
    {
        Navigation.NavigateTo("/chat/123e4567-e89b-12d3-a456-426614174000");
    }
}
```

**Compare to React Router**:
```tsx
// React
<Route path="/chat/:conversationId" element={<ChatPage />} />
const { conversationId } = useParams();
const navigate = useNavigate();
navigate('/chat/123');
```

**Blazor routing is simpler** - no extra libraries, built-in.

---

### 8. Forms & Validation

Blazor has built-in form handling:

```razor
<EditForm Model="@loginModel" OnValidSubmit="HandleLogin">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <div>
        <label>Email:</label>
        <InputText @bind-Value="loginModel.Email" />
        <ValidationMessage For="@(() => loginModel.Email)" />
    </div>

    <div>
        <label>Password:</label>
        <InputText @bind-Value="loginModel.Password" type="password" />
        <ValidationMessage For="@(() => loginModel.Password)" />
    </div>

    <button type="submit">Login</button>
</EditForm>

@code {
    private LoginModel loginModel = new();

    private async Task HandleLogin()
    {
        // Only runs if model is valid
        await AuthService.LoginAsync(loginModel);
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = "";

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; } = "";
    }
}
```

**Key features**:
- `EditForm` - Smart form wrapper
- `DataAnnotationsValidator` - Uses SAME validation as backend
- `ValidationMessage` - Shows errors per field
- `ValidationSummary` - Shows all errors
- `OnValidSubmit` - Only fires if form is valid

**Compare to React**:
- React: Need library (Formik, React Hook Form, Yup)
- Blazor: Built-in, uses same validation as backend

**HUGE advantage**: Share validation between frontend/backend!

---

### 9. HTTP Calls (Calling Your API)

**Setup (Program.cs)**:
```csharp
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:5000")
});
```

**Use in components**:
```razor
@inject HttpClient Http

@code {
    private async Task<Result<UserDto>> GetUserAsync(Guid id)
    {
        // GET request
        var user = await Http.GetFromJsonAsync<UserDto>($"api/users/{id}");

        // POST request
        var result = await Http.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123"
        });

        return await result.Content.ReadFromJsonAsync<Result<UserDto>>();
    }
}
```

**With your Result<T> pattern**:
```razor
@code {
    private string? errorMessage;
    private UserDto? user;

    private async Task LoadUser()
    {
        var response = await Http.GetAsync("api/users/123");
        var result = await response.Content.ReadFromJsonAsync<Result<UserDto>>();

        if (result.IsSuccess)
        {
            user = result.Value;
        }
        else
        {
            errorMessage = result.Error;
        }
    }
}
```

**No axios, no fetch, no manual JSON parsing** - it's built into HttpClient.

---

### 10. Conditional Rendering & Loops

**If/Else**:
```razor
@if (isLoading)
{
    <p>Loading...</p>
}
else if (errorMessage != null)
{
    <div class="error">@errorMessage</div>
}
else
{
    <div class="content">@data</div>
}
```

**Loops**:
```razor
<ul>
    @foreach (var user in users)
    {
        <li>@user.Name - @user.Email</li>
    }
</ul>
```

**With components**:
```razor
@foreach (var conversation in conversations)
{
    <ConversationCard
        Title="@conversation.Title"
        OnClick="@(() => SelectConversation(conversation.Id))"
    />
}
```

**Compare to React**:
```tsx
// React
{isLoading ? <p>Loading...</p> : <div>{data}</div>}
{users.map(user => <li key={user.id}>{user.name}</li>)}
```

**Blazor feels more like C#** (because it IS C#).

---

## Razor Syntax Quick Reference

| Purpose | Syntax | Example |
|---------|--------|---------|
| Output C# expression | `@expression` | `<p>@username</p>` |
| Code block | `@{ code }` | `@{ var x = 5; }` |
| If statement | `@if (condition) { }` | `@if (isLoggedIn) { <p>Welcome!</p> }` |
| Loop | `@foreach (var item in items) { }` | `@foreach (var u in users) { <p>@u.Name</p> }` |
| Event handler | `@onclick="Method"` | `<button @onclick="Save">Save</button>` |
| Two-way binding | `@bind="field"` | `<input @bind="username" />` |
| Component parameter | `Name="@value"` | `<Card Title="@title" />` |
| Inject service | `@inject IService Service` | `@inject HttpClient Http` |
| Define route | `@page "/path"` | `@page "/users/{id:int}"` |
| Comment | `@* comment *@` | `@* This is a comment *@` |

---

## Key Differences from React (Mental Model Shift)

### 1. Explicit Re-rendering
- **React**: Changes state → automatic re-render
- **Blazor**: Changes state → call `StateHasChanged()` → re-render

**Why**: Blazor gives you control. No mysterious re-renders.

### 2. No Virtual DOM Diffing Complexity
- **React**: Virtual DOM, reconciliation, keys, `React.memo`, `useMemo`
- **Blazor**: Simple diff, just re-render what changed

**Why**: Less mental overhead.

### 3. Strongly Typed Everything
- **React**: Props are `any` at runtime (even with TypeScript)
- **Blazor**: Props are C# types, validated at compile time

**Why**: Catch bugs before runtime.

### 4. Lifecycle is Predictable
- **React**: `useEffect` dependency arrays, closure issues, stale closures
- **Blazor**: Methods called in order, no closures, no surprises

**Why**: No brain freeze when debugging.

### 5. Forms are First-Class
- **React**: Need libraries (Formik, React Hook Form)
- **Blazor**: Built-in `EditForm`, uses DataAnnotations

**Why**: Share validation with backend.

---

## Common Patterns You'll Use

### Pattern 1: Loading Data on Init
```razor
@code {
    private List<ConversationDto> conversations = new();
    private bool isLoading = true;
    private string? error;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var result = await Http.GetFromJsonAsync<Result<List<ConversationDto>>>("api/conversations");

            if (result.IsSuccess)
            {
                conversations = result.Value;
            }
            else
            {
                error = result.Error;
            }
        }
        catch (Exception ex)
        {
            error = ex.Message;
        }
        finally
        {
            isLoading = false;
        }
    }
}
```

### Pattern 2: Parent-Child Communication
**Parent**:
```razor
<ChildComponent OnDataChanged="HandleDataChanged" />

@code {
    private void HandleDataChanged(string newData)
    {
        Console.WriteLine($"Child says: {newData}");
    }
}
```

**Child**:
```razor
<button @onclick="NotifyParent">Click</button>

@code {
    [Parameter]
    public EventCallback<string> OnDataChanged { get; set; }

    private async Task NotifyParent()
    {
        await OnDataChanged.InvokeAsync("Hello from child!");
    }
}
```

### Pattern 3: Conditional Rendering with Loading/Error States
```razor
@if (isLoading)
{
    <LoadingSpinner />
}
else if (error != null)
{
    <ErrorMessage Message="@error" />
}
else if (data == null)
{
    <p>No data available</p>
}
else
{
    <DataDisplay Data="@data" />
}
```

---

## Next Steps

Now that you understand the fundamentals:
1. **Read**: Blazor-02-Project-Structure.md - Learn how to organize a real Blazor app
2. **Read**: Blazor-03-Component-Patterns.md - Advanced patterns for complex UIs
3. **Read**: Blazor-04-Clean-Architecture.md - Integrate Blazor with your backend architecture

---

## Key Takeaways

✅ Blazor components are `.razor` files with markup + C# code
✅ Use `@` for C# expressions, `@code` for logic
✅ Parameters = props, marked with `[Parameter]`
✅ Lifecycle is explicit and predictable
✅ Re-rendering is explicit (`StateHasChanged()`)
✅ DI works exactly like backend
✅ Forms use DataAnnotations (same as backend)
✅ HttpClient is built-in, no axios needed
✅ **Everything feels like C# because it IS C#**

**Bottom line**: If you know C# and ASP.NET Core, you already know 80% of Blazor.
