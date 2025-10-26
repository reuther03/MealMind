# Blazor Component Patterns & Best Practices

## Common Component Patterns

### Pattern 1: Smart vs Presentational Components

Like React, separate components into two types:

**Smart Components** (Pages):
- Connected to services
- Manage state
- Handle business logic
- Coordinate child components

**Presentational Components**:
- Receive data via `[Parameter]`
- Emit events via `EventCallback`
- No service dependencies
- Pure UI logic

---

**Example: Smart Component (Page)**

`Pages/Chat/ChatRoom.razor`:
```razor
@page "/chat/{ConversationId:guid}"
@inject IChatService ChatService
@inject AuthState AuthState

<div class="chat-room">
    <ChatHeader Title="@conversation?.Title" />

    <MessageList
        Messages="@messages"
        CurrentUserId="@AuthState.CurrentUser?.Id"
    />

    <ChatInput
        IsLoading="@isLoading"
        OnSendMessage="HandleSendMessage"
    />
</div>

@code {
    [Parameter]
    public Guid ConversationId { get; set; }

    private ConversationDto? conversation;
    private List<MessageDto> messages = new();
    private bool isLoading;

    protected override async Task OnInitializedAsync()
    {
        await LoadConversation();
    }

    private async Task LoadConversation()
    {
        var result = await ChatService.GetConversationAsync(ConversationId);

        if (result.IsSuccess)
        {
            conversation = result.Value;
            messages = result.Value.Messages;
        }
    }

    private async Task HandleSendMessage(string prompt)
    {
        isLoading = true;

        var result = await ChatService.SendMessageAsync(ConversationId, prompt);

        if (result.IsSuccess)
        {
            messages.Add(result.Value);
        }

        isLoading = false;
    }
}
```

**Presentational Component**

`Components/Chat/ChatInput.razor`:
```razor
<div class="chat-input">
    <textarea
        @bind="currentMessage"
        @bind:event="oninput"
        placeholder="Type your message..."
        disabled="@IsLoading"
    />

    <button
        @onclick="SendMessage"
        disabled="@(IsLoading || string.IsNullOrWhiteSpace(currentMessage))"
    >
        @if (IsLoading)
        {
            <span>Sending...</span>
        }
        else
        {
            <span>Send</span>
        }
    </button>
</div>

@code {
    [Parameter]
    public bool IsLoading { get; set; }

    [Parameter]
    public EventCallback<string> OnSendMessage { get; set; }

    private string currentMessage = "";

    private async Task SendMessage()
    {
        if (string.IsNullOrWhiteSpace(currentMessage)) return;

        await OnSendMessage.InvokeAsync(currentMessage);
        currentMessage = ""; // Clear input after send
    }
}
```

**Benefits**:
- ✅ Presentational components are reusable
- ✅ Easy to test (just pass props)
- ✅ Clear separation of concerns

---

### Pattern 2: Loading/Error/Empty States

Always handle these three states:

```razor
@if (isLoading)
{
    <LoadingState />
}
else if (error != null)
{
    <ErrorState Message="@error" OnRetry="LoadData" />
}
else if (!items.Any())
{
    <EmptyState Message="No conversations yet" OnAction="CreateNew" />
}
else
{
    <DataDisplay Items="@items" />
}

@code {
    private List<ConversationDto> items = new();
    private bool isLoading = true;
    private string? error;

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        isLoading = true;
        error = null;

        var result = await ChatService.GetConversationsAsync();

        if (result.IsSuccess)
        {
            items = result.Value;
        }
        else
        {
            error = result.Error;
        }

        isLoading = false;
    }
}
```

**Create reusable state components**:

`Components/Common/LoadingState.razor`:
```razor
<div class="loading-state">
    <div class="spinner"></div>
    <p>Loading...</p>
</div>
```

`Components/Common/ErrorState.razor`:
```razor
<div class="error-state">
    <p class="error-message">@Message</p>
    @if (OnRetry.HasDelegate)
    {
        <button @onclick="OnRetry">Try Again</button>
    }
</div>

@code {
    [Parameter, EditorRequired]
    public string Message { get; set; } = "";

    [Parameter]
    public EventCallback OnRetry { get; set; }
}
```

---

### Pattern 3: Async Operations with Cancellation

When component can be disposed while async operation is running:

```razor
@implements IDisposable

@code {
    private CancellationTokenSource? _cts;

    protected override async Task OnInitializedAsync()
    {
        _cts = new CancellationTokenSource();

        try
        {
            await LoadDataAsync(_cts.Token);
        }
        catch (OperationCanceledException)
        {
            // Component was disposed, ignore
        }
    }

    private async Task LoadDataAsync(CancellationToken ct)
    {
        // Long-running operation
        await Task.Delay(5000, ct);
        var result = await ChatService.GetDataAsync(ct);
        // ...
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}
```

**When to use**:
- ✅ API calls that might take a while
- ✅ Polling/timers
- ✅ WebSocket connections

---

### Pattern 4: Debouncing User Input

For search boxes or autosave:

```razor
<input @bind="searchQuery" @bind:event="oninput" placeholder="Search..." />

@code {
    private string _searchQuery = "";
    private Timer? _debounceTimer;

    private string searchQuery
    {
        get => _searchQuery;
        set
        {
            _searchQuery = value;
            DebounceSearch();
        }
    }

    private void DebounceSearch()
    {
        _debounceTimer?.Dispose();
        _debounceTimer = new Timer(async _ =>
        {
            await InvokeAsync(async () =>
            {
                await PerformSearch(_searchQuery);
                StateHasChanged();
            });
        }, null, 500, Timeout.Infinite); // 500ms delay
    }

    private async Task PerformSearch(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return;

        var results = await SearchService.SearchAsync(query);
        // Update UI
    }

    public void Dispose()
    {
        _debounceTimer?.Dispose();
    }
}
```

**Alternative: Using Task.Delay**:
```razor
@code {
    private string searchQuery = "";
    private CancellationTokenSource? _searchCts;

    private async Task OnSearchChanged(ChangeEventArgs e)
    {
        searchQuery = e.Value?.ToString() ?? "";

        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();

        try
        {
            await Task.Delay(500, _searchCts.Token); // Debounce
            await PerformSearch(searchQuery);
        }
        catch (OperationCanceledException)
        {
            // User typed again, ignore
        }
    }
}
```

---

### Pattern 5: Optimistic UI Updates

Update UI immediately, then confirm with server:

```razor
@code {
    private async Task DeleteConversation(Guid id)
    {
        // 1. Optimistically remove from UI
        var conversation = conversations.FirstOrDefault(c => c.Id == id);
        if (conversation != null)
        {
            conversations.Remove(conversation);
            StateHasChanged(); // Update UI immediately
        }

        // 2. Confirm with server
        var result = await ChatService.DeleteConversationAsync(id);

        // 3. If failed, rollback
        if (!result.IsSuccess)
        {
            if (conversation != null)
            {
                conversations.Add(conversation);
                StateHasChanged();
            }

            ShowError(result.Error);
        }
    }
}
```

**When to use**:
- ✅ Delete operations (feels instant)
- ✅ Toggle states (like/favorite)
- ✅ Sorting/filtering locally

---

### Pattern 6: Cascading Parameters (Avoid Prop Drilling)

Pass data down multiple levels without threading through every component:

**Parent (Top Level)**:
```razor
<CascadingValue Value="@authState">
    <Router ... />
</CascadingValue>

@code {
    private AuthState authState = new();
}
```

**Deep Child Component**:
```razor
@code {
    [CascadingParameter]
    private AuthState AuthState { get; set; } = null!;

    // Now you can use AuthState without passing through intermediate components
    private string Username => AuthState.CurrentUser?.Name ?? "Guest";
}
```

**When to use**:
- ✅ Auth state (current user, token)
- ✅ Theme settings (dark mode)
- ✅ Localization (language)

**When NOT to use**:
- ❌ Data that changes frequently (causes all descendants to re-render)
- ❌ Component-specific data (use parameters instead)

---

### Pattern 7: Component Composition

Build complex UIs from small pieces:

**Slot Pattern (RenderFragment)**:
```razor
@* Card.razor *@
<div class="card">
    <div class="card-header">
        @Header
    </div>

    <div class="card-body">
        @ChildContent
    </div>

    @if (Footer != null)
    {
        <div class="card-footer">
            @Footer
        </div>
    }
</div>

@code {
    [Parameter]
    public RenderFragment? Header { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public RenderFragment? Footer { get; set; }
}
```

**Usage**:
```razor
<Card>
    <Header>
        <h2>Conversation Title</h2>
    </Header>

    <ChildContent>
        <p>This is the main content</p>
    </ChildContent>

    <Footer>
        <button>Delete</button>
    </Footer>
</Card>
```

**Benefits**:
- ✅ Flexible layouts
- ✅ Reusable wrappers
- ✅ Clean component API

---

### Pattern 8: Conditional CSS Classes

Apply classes based on state:

```razor
<div class="message @GetMessageClass()">
    @Content
</div>

@code {
    [Parameter]
    public bool IsCurrentUser { get; set; }

    [Parameter]
    public bool IsError { get; set; }

    private string GetMessageClass()
    {
        var classes = new List<string>();

        if (IsCurrentUser)
            classes.Add("message-sent");
        else
            classes.Add("message-received");

        if (IsError)
            classes.Add("message-error");

        return string.Join(" ", classes);
    }
}
```

**Alternative (inline)**:
```razor
<div class="message @(IsCurrentUser ? "sent" : "received") @(IsError ? "error" : "")">
    @Content
</div>
```

**Using helper method**:
```csharp
public static class CssHelper
{
    public static string Class(params (bool condition, string className)[] conditions)
    {
        return string.Join(" ", conditions.Where(c => c.condition).Select(c => c.className));
    }
}

// Usage:
<div class="@CssHelper.Class((IsActive, "active"), (IsSelected, "selected"))">
```

---

### Pattern 9: Modal Dialog Pattern

Reusable modal component:

`Components/Common/Modal.razor`:
```razor
@if (IsVisible)
{
    <div class="modal-backdrop" @onclick="HandleBackdropClick">
        <div class="modal-content" @onclick:stopPropagation>
            <div class="modal-header">
                <h3>@Title</h3>
                <button @onclick="Close">&times;</button>
            </div>

            <div class="modal-body">
                @ChildContent
            </div>

            @if (Footer != null)
            {
                <div class="modal-footer">
                    @Footer
                </div>
            }
        </div>
    </div>
}

@code {
    [Parameter]
    public string Title { get; set; } = "";

    [Parameter]
    public bool IsVisible { get; set; }

    [Parameter]
    public bool CloseOnBackdropClick { get; set; } = true;

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public RenderFragment? Footer { get; set; }

    private async Task Close()
    {
        await OnClose.InvokeAsync();
    }

    private async Task HandleBackdropClick()
    {
        if (CloseOnBackdropClick)
        {
            await Close();
        }
    }
}
```

**Usage**:
```razor
<button @onclick="() => showModal = true">Open Modal</button>

<Modal
    Title="Delete Conversation"
    IsVisible="@showModal"
    OnClose="() => showModal = false"
>
    <ChildContent>
        <p>Are you sure you want to delete this conversation?</p>
    </ChildContent>

    <Footer>
        <button @onclick="ConfirmDelete">Delete</button>
        <button @onclick="() => showModal = false">Cancel</button>
    </Footer>
</Modal>

@code {
    private bool showModal = false;

    private async Task ConfirmDelete()
    {
        await ChatService.DeleteAsync(conversationId);
        showModal = false;
    }
}
```

---

### Pattern 10: List Virtualization (For Long Lists)

Blazor has built-in virtualization for performance:

```razor
@using Microsoft.AspNetCore.Components.Web.Virtualization

<Virtualize Items="@messages" Context="message">
    <MessageBubble Message="@message" />
</Virtualize>

@code {
    private List<MessageDto> messages = new();
}
```

**For async loading**:
```razor
<Virtualize ItemsProvider="@LoadMessages" Context="message">
    <ItemContent>
        <MessageBubble Message="@message" />
    </ItemContent>
    <Placeholder>
        <p>Loading...</p>
    </Placeholder>
</Virtualize>

@code {
    private async ValueTask<ItemsProviderResult<MessageDto>> LoadMessages(ItemsProviderRequest request)
    {
        var result = await ChatService.GetMessagesAsync(
            skip: request.StartIndex,
            take: request.Count
        );

        return new ItemsProviderResult<MessageDto>(
            items: result.Value,
            totalItemCount: result.TotalCount
        );
    }
}
```

**When to use**:
- ✅ Lists with 100+ items
- ✅ Infinite scroll
- ✅ Performance-critical rendering

---

## Best Practices

### 1. Keep Components Small

**Target**: <150 lines per component

**If too large, split**:
```razor
@* Before: ChatRoom.razor (400 lines) *@

@* After: *@
ChatRoom.razor (100 lines) - orchestrates
  ├─ ChatHeader.razor (30 lines)
  ├─ MessageList.razor (80 lines)
  │   └─ MessageBubble.razor (40 lines)
  └─ ChatInput.razor (60 lines)
```

---

### 2. Use `[EditorRequired]` for Required Parameters

```csharp
[Parameter, EditorRequired]
public ConversationDto Conversation { get; set; } = null!;
```

**Benefits**:
- ✅ Compile warning if missing
- ✅ Self-documenting
- ✅ Prevents runtime null errors

---

### 3. Dispose Resources

Always implement `IDisposable` if you:
- Subscribe to events
- Use timers
- Create HttpClient
- Use CancellationTokenSource

```razor
@implements IDisposable

@code {
    protected override void OnInitialized()
    {
        AuthState.OnChange += StateHasChanged;
    }

    public void Dispose()
    {
        AuthState.OnChange -= StateHasChanged;
    }
}
```

---

### 4. Avoid `async void` Event Handlers

❌ **Bad**:
```razor
<button @onclick="HandleClick">Click</button>

@code {
    private async void HandleClick() // DON'T USE async void
    {
        await DoSomethingAsync();
    }
}
```

✅ **Good**:
```razor
<button @onclick="HandleClick">Click</button>

@code {
    private async Task HandleClick() // Use async Task
    {
        await DoSomethingAsync();
    }
}
```

**Why**: `async void` swallows exceptions, can't be awaited, harder to test.

---

### 5. Use `@key` for Dynamic Lists

When rendering lists that can reorder:

```razor
@foreach (var message in messages)
{
    <MessageBubble @key="message.Id" Message="@message" />
}
```

**Why**: Tells Blazor how to match components on re-render (like React's `key`).

---

### 6. Extract Repeated Logic to Services

❌ **Bad** (logic in component):
```razor
@code {
    private async Task SendMessage(string prompt)
    {
        var response = await Http.PostAsJsonAsync(...);
        var result = await response.Content.ReadFromJsonAsync<Result<MessageDto>>();

        if (result.IsSuccess)
        {
            messages.Add(result.Value);
        }
    }
}
```

✅ **Good** (logic in service):
```razor
@inject IChatService ChatService

@code {
    private async Task SendMessage(string prompt)
    {
        var result = await ChatService.SendMessageAsync(conversationId, prompt);

        if (result.IsSuccess)
        {
            messages.Add(result.Value);
        }
    }
}
```

---

### 7. Use CSS Isolation

**Component**: `MessageBubble.razor`
```razor
<div class="bubble">
    <p>@Content</p>
</div>
```

**Scoped CSS**: `MessageBubble.razor.css`
```css
.bubble {
    padding: 12px;
    border-radius: 8px;
    background: #f0f0f0;
}

.bubble p {
    margin: 0;
}
```

**Result**: Styles ONLY apply to `MessageBubble`, won't leak to other components.

---

### 8. Avoid `InvokeAsync` Unless Needed

❌ **Not needed** (already on Blazor thread):
```csharp
private async Task OnButtonClick()
{
    await InvokeAsync(async () => // Unnecessary
    {
        await LoadDataAsync();
    });
}
```

✅ **Needed** (called from non-Blazor thread):
```csharp
private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
{
    InvokeAsync(async () => // NEEDED - timer callback is not Blazor thread
    {
        await LoadDataAsync();
        StateHasChanged();
    });
}
```

**When to use `InvokeAsync`**:
- ✅ Timer callbacks
- ✅ Event handlers from external libraries
- ✅ Background thread operations

---

### 9. Prefer `OnInitializedAsync` Over `OnInitialized`

If you need async, use the async version:

✅ **Good**:
```csharp
protected override async Task OnInitializedAsync()
{
    await LoadDataAsync();
}
```

❌ **Bad**:
```csharp
protected override void OnInitialized()
{
    _ = LoadDataAsync(); // Fire and forget, bad practice
}
```

---

### 10. Null-Forgiving Operator for Required Parameters

When you KNOW parameter will be set:

```csharp
[Parameter, EditorRequired]
public ConversationDto Conversation { get; set; } = null!; // null-forgiving
```

**Why**: Prevents nullable warnings while maintaining type safety.

---

## Common Pitfalls

### Pitfall 1: Forgetting `StateHasChanged()`

**Symptom**: UI doesn't update after async operation

```csharp
private async Task LoadData()
{
    await Task.Delay(1000);
    data = "New data";
    // UI doesn't update! Need StateHasChanged()
}
```

**Fix**:
```csharp
private async Task LoadData()
{
    await Task.Delay(1000);
    data = "New data";
    StateHasChanged(); // ← Add this
}
```

---

### Pitfall 2: Mutating Collections Without New Reference

**Symptom**: List updated but UI doesn't re-render

```csharp
messages.Add(newMessage); // Mutates, but Blazor doesn't detect
```

**Fix 1** (call StateHasChanged):
```csharp
messages.Add(newMessage);
StateHasChanged();
```

**Fix 2** (create new list):
```csharp
messages = messages.Append(newMessage).ToList(); // New reference
```

---

### Pitfall 3: Event Handler Memory Leaks

**Problem**:
```csharp
protected override void OnInitialized()
{
    AuthState.OnChange += StateHasChanged;
    // If you forget to unsubscribe, component stays in memory even when removed
}
```

**Fix**:
```csharp
@implements IDisposable

protected override void OnInitialized()
{
    AuthState.OnChange += StateHasChanged;
}

public void Dispose()
{
    AuthState.OnChange -= StateHasChanged; // ← Unsubscribe
}
```

---

### Pitfall 4: Not Handling Null Parameters

**Problem**:
```razor
<p>@Conversation.Title</p> @* NullReferenceException if Conversation is null *@

@code {
    [Parameter]
    public ConversationDto? Conversation { get; set; }
}
```

**Fix**:
```razor
@if (Conversation != null)
{
    <p>@Conversation.Title</p>
}
else
{
    <p>Loading conversation...</p>
}
```

---

## Performance Tips

### 1. Use `ShouldRender()` to Skip Unnecessary Renders

```csharp
protected override bool ShouldRender()
{
    // Only re-render if data actually changed
    return _previousData != currentData;
}
```

**Use sparingly** - premature optimization.

---

### 2. Memoize Expensive Computations

```csharp
private string? _cachedSummary;
private List<MessageDto>? _cachedMessages;

private string Summary
{
    get
    {
        if (_cachedMessages != messages)
        {
            _cachedMessages = messages;
            _cachedSummary = GenerateSummary(messages); // Expensive
        }
        return _cachedSummary ?? "";
    }
}
```

---

### 3. Lazy Load Components

```razor
@if (showAdvancedSettings)
{
    <AdvancedSettings /> @* Only loads when needed *@
}
```

---

## Testing Components (Brief Overview)

Blazor components are testable with **bUnit**:

```csharp
[Fact]
public void MessageBubble_DisplaysContent()
{
    // Arrange
    using var ctx = new TestContext();
    var message = new MessageDto { Content = "Hello, world!" };

    // Act
    var cut = ctx.RenderComponent<MessageBubble>(parameters => parameters
        .Add(p => p.Message, message)
    );

    // Assert
    cut.Find("p").TextContent.Should().Be("Hello, world!");
}
```

**Resources**: [bUnit Documentation](https://bunit.dev)

---

## Next Steps

Now that you know component patterns:
1. **Read**: Blazor-04-Clean-Architecture.md - How to structure frontend like your backend
2. **Practice**: Build a simple component (MessageBubble, LoadingSpinner)
3. **Experiment**: Try different patterns with MealMind features

---

## Key Takeaways

✅ Separate smart (pages) from presentational (components)
✅ Always handle loading/error/empty states
✅ Use `EventCallback` for parent-child communication
✅ Dispose subscriptions to avoid memory leaks
✅ Keep components small (<150 lines)
✅ Use CSS isolation for scoped styles
✅ Use `@key` for dynamic lists
✅ Extract logic to services
✅ Use `async Task`, not `async void`
✅ Call `StateHasChanged()` after manual async operations

**Bottom line**: Blazor components follow predictable patterns - once you learn them, building UIs feels natural.
