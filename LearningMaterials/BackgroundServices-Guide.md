# Background Services in .NET - Patterns & Practices

## Overview

Background services run tasks independently of HTTP requests. Use them for:
- Scheduled jobs (daily cleanup, maintenance)
- Queue processing
- Long-running tasks
- Periodic data sync

---

## 1. Basic Implementation

### Simple BackgroundService

```csharp
public class SimpleBackgroundService : BackgroundService
{
    private readonly ILogger<SimpleBackgroundService> _logger;

    public SimpleBackgroundService(ILogger<SimpleBackgroundService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {Time}", DateTime.UtcNow);

            // Do work here

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}

// Registration in Program.cs or module
services.AddHostedService<SimpleBackgroundService>();
```

**When to use:** Simple periodic tasks, prototypes, low-stakes jobs.

**Limitations:** No persistence, no retry logic, no dashboard.

---

## 2. Production-Ready Patterns

### Pattern 1: Scoped Service Access

**Problem:** `BackgroundService` is singleton, but `DbContext` and repositories are scoped.

```csharp
// ❌ WRONG - Cannot inject scoped services into singleton
public class BadService : BackgroundService
{
    private readonly IRepository _repo; // Scoped service - will throw!

    public BadService(IRepository repo)
    {
        _repo = repo;
    }
}

// ✅ CORRECT - Create scope for each unit of work
public class GoodService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<GoodService> _logger;

    public GoodService(IServiceScopeFactory scopeFactory, ILogger<GoodService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await DoWorkAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task DoWorkAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Do work with scoped services
        await unitOfWork.CommitAsync(ct);
    }
    // Scope is disposed here - DbContext is released
}
```

**Rule:** Always use `IServiceScopeFactory` when accessing scoped services.

---

### Pattern 2: Exception Handling (Critical!)

**Problem:** Unhandled exception kills the background service silently.

```csharp
// ❌ WRONG - Service dies on first exception
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    while (!stoppingToken.IsCancellationRequested)
    {
        await DoWorkAsync(); // If this throws, service stops forever
        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
    }
}

// ✅ CORRECT - Catch, log, continue
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    while (!stoppingToken.IsCancellationRequested)
    {
        try
        {
            await DoWorkAsync(stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Graceful shutdown - don't log as error
            _logger.LogInformation("Service is stopping");
            break;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in background service. Will retry in 5 minutes.");
            // Service continues running
        }

        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
    }
}
```

**Rule:** Never let exceptions escape `ExecuteAsync` unless you want the service to stop.

---

### Pattern 3: PeriodicTimer (Modern .NET 6+)

```csharp
public class ModernTimerService : BackgroundService
{
    private readonly PeriodicTimer _timer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ModernTimerService> _logger;

    public ModernTimerService(IServiceScopeFactory scopeFactory, ILogger<ModernTimerService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _timer = new PeriodicTimer(TimeSpan.FromHours(1));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Run immediately on startup, then on timer
        await DoWorkAsync(stoppingToken);

        while (await _timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await DoWorkAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in periodic job");
            }
        }
    }

    private async Task DoWorkAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        // Work...
    }

    public override void Dispose()
    {
        _timer.Dispose();
        base.Dispose();
    }
}
```

**Benefits over while+delay:**
- More precise timing
- Cleaner cancellation
- No drift over time

---

### Pattern 4: Run at Specific Time

```csharp
public class MidnightJobService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = CalculateDelayUntilMidnight();

            _logger.LogInformation("Next run in {Delay}", delay);

            await Task.Delay(delay, stoppingToken);

            if (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await DoWorkAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Midnight job failed");
                }
            }
        }
    }

    private TimeSpan CalculateDelayUntilMidnight()
    {
        var now = DateTime.UtcNow;
        var nextMidnight = now.Date.AddDays(1); // Next UTC midnight
        return nextMidnight - now;
    }
}
```

---

### Pattern 5: Startup Delay (Don't Block App Startup)

```csharp
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    // Wait for app to fully start before doing heavy work
    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

    // Now start the main loop
    while (!stoppingToken.IsCancellationRequested)
    {
        // ...
    }
}
```

**Why:** Heavy work during startup can slow down app initialization and health checks.

---

## 3. Your DailyLog Use Case

### Production-Ready Implementation

```csharp
public class DailyLogMaintenanceService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DailyLogMaintenanceService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(6);
    private readonly int _daysToCreate = 7;

    public DailyLogMaintenanceService(
        IServiceScopeFactory scopeFactory,
        ILogger<DailyLogMaintenanceService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Delay startup to not interfere with app initialization
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CreateMissingDailyLogsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create daily logs. Will retry.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task CreateMissingDailyLogsAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var userRepo = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var dailyLogRepo = scope.ServiceProvider.GetRequiredService<IDailyLogRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var users = await userRepo.GetAllActiveAsync(ct);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        foreach (var user in users)
        {
            for (int i = 0; i < _daysToCreate; i++)
            {
                var date = today.AddDays(i);
                var exists = await dailyLogRepo.ExistsAsync(user.Id, date, ct);

                if (!exists)
                {
                    var dailyLog = DailyLog.Create(user.Id, date, user.GetNutritionTargetFor(date));
                    await dailyLogRepo.AddAsync(dailyLog, ct);

                    _logger.LogDebug("Created DailyLog for {UserId} on {Date}", user.Id, date);
                }
            }
        }

        await unitOfWork.CommitAsync(ct);
        _logger.LogInformation("Daily log maintenance completed for {Count} users", users.Count);
    }
}
```

**This is enough for your project.** Simple, reliable, easy to understand.

---

## 4. When to Use External Libraries

### Hangfire

**Use when you need:**
- Web dashboard to monitor jobs
- Automatic retries with backoff
- Job persistence (survives app restart)
- Delayed jobs ("send email in 1 hour")
- Multiple server instances processing same queue

```csharp
// Hangfire example
services.AddHangfire(x => x.UsePostgreSqlStorage(connectionString));
services.AddHangfireServer();

// Schedule recurring job
RecurringJob.AddOrUpdate<IDailyLogService>(
    "daily-log-maintenance",
    service => service.CreateMissingLogsAsync(),
    Cron.Daily(0, 0)); // Every day at midnight
```

**Overkill when:** Single instance app, simple periodic tasks, no need for dashboard.

---

### Quartz.NET

**Use when you need:**
- Complex cron schedules
- Job clustering across multiple instances
- Trigger dependencies
- Calendar-based scheduling

```csharp
// Quartz example
services.AddQuartz(q =>
{
    var jobKey = new JobKey("DailyLogJob");

    q.AddJob<DailyLogJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithCronSchedule("0 0 0 * * ?")); // Midnight every day
});

services.AddQuartzHostedService();
```

**Overkill when:** Simple time-based scheduling, no clustering needs.

---

## 5. Decision Matrix

| Scenario | Solution | Why |
|----------|----------|-----|
| Simple periodic task (every X hours) | `BackgroundService` | Built-in, simple, no dependencies |
| Run at specific time (midnight) | `BackgroundService` with delay calculation | Still simple enough |
| Need retries on failure | `BackgroundService` + try/catch + counter | Simple manual retry |
| Need job persistence | **Hangfire** | Jobs survive restart |
| Need dashboard/monitoring | **Hangfire** | Built-in dashboard |
| Complex cron expressions | **Quartz.NET** | Full cron support |
| Multiple app instances, single job execution | **Hangfire** or **Quartz.NET** | Distributed locking |
| Queue processing (async jobs) | **Hangfire** or custom with Redis | Job queuing |

---

## 6. Common Mistakes

### Mistake 1: Blocking ExecuteAsync

```csharp
// ❌ WRONG - Blocks app startup
protected override Task ExecuteAsync(CancellationToken stoppingToken)
{
    while (!stoppingToken.IsCancellationRequested)
    {
        Thread.Sleep(5000); // Blocking!
    }
    return Task.CompletedTask;
}

// ✅ CORRECT - Async all the way
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    while (!stoppingToken.IsCancellationRequested)
    {
        await Task.Delay(5000, stoppingToken);
    }
}
```

### Mistake 2: Not Respecting CancellationToken

```csharp
// ❌ WRONG - Ignores shutdown signal
await Task.Delay(TimeSpan.FromHours(1)); // No token!

// ✅ CORRECT - Graceful shutdown
await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
```

### Mistake 3: Fire-and-Forget Without Logging

```csharp
// ❌ WRONG - Silent failures
_ = DoSomethingAsync();

// ✅ CORRECT - At minimum log errors
_ = Task.Run(async () =>
{
    try
    {
        await DoSomethingAsync();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Fire-and-forget task failed");
    }
});
```

---

## 7. Summary

**For MealMind (and most small-medium apps):**

1. Use built-in `BackgroundService`
2. Always use `IServiceScopeFactory` for scoped services
3. Always wrap work in try/catch
4. Use `PeriodicTimer` for clean periodic jobs
5. Add startup delay to not block app initialization

**Add Hangfire/Quartz only when:**
- You need job persistence
- You need a monitoring dashboard
- You have multiple instances needing coordination
- You have complex scheduling requirements

**Don't over-engineer.** A simple `BackgroundService` with proper error handling is production-ready for most scenarios.
