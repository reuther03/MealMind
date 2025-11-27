# 🚀 Week 1: Getting to Production

**Goal:** By end of this week, your API will be running on a real server, accessible from the internet!

**Estimated Total Time:** 15-20 hours (2-3 hours per day)

---

## 📅 Daily Breakdown

### Monday: Improve ImageAnalyzeCreatedEventHandler (Already Implemented!)

**Time:** 30 minutes - 1 hour
**Difficulty:** ⭐☆☆☆☆ (Very Easy - just improvements)

✅ **Good news:** You already implemented this! Just need small improvements.

#### Tasks:

**Task 1.1: Add Logging and Error Handling** (20 minutes)

**File:** `src/Modules/Nutrition/MealMind.Modules.Nutrition.Application/Events/Integration/ImageAnalyzeCreatedEventHandler.cs`

**Current State:**
```csharp
public async Task Handle(ImageAnalyzeCreatedEvent notification, CancellationToken cancellationToken)
{
    var dailyLog = await _dailyLogRepository.GetByDateAsync(notification.DailyLogDate, notification.UserId, cancellationToken);
    if (dailyLog == null)
        throw new ApplicationException($"Daily log for date {notification.DailyLogDate} not found.");  // ⚠️ Throws exception

    var foodEntry = FoodEntry.CreateFromImageAnalyze(...);
    var meal = dailyLog.Meals[0];  // ⚠️ Could throw if empty
    meal.AddFood(foodEntry);
    await _unitOfWork.CommitAsync(cancellationToken);
}
```

**What to improve:**
**Just add logging and improve error handling:**

```csharp
using Microsoft.Extensions.Logging;  // ← Add this

public class ImageAnalyzeCreatedEventHandler : INotificationHandler<ImageAnalyzeCreatedEvent>
{
    private readonly IDailyLogRepository _dailyLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ImageAnalyzeCreatedEventHandler> _logger;  // ← Add this

    public ImageAnalyzeCreatedEventHandler(
        IDailyLogRepository dailyLogRepository,
        IUnitOfWork unitOfWork,
        ILogger<ImageAnalyzeCreatedEventHandler> logger)  // ← Add this
    {
        _dailyLogRepository = dailyLogRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;  // ← Add this
    }

    public async Task Handle(ImageAnalyzeCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing ImageAnalyzeCreatedEvent for user {UserId}, food {FoodName}",
            notification.UserId,
            notification.FoodName);

        var dailyLog = await _dailyLogRepository.GetByDateAsync(
            notification.DailyLogDate,
            notification.UserId,
            cancellationToken);

        if (dailyLog == null)
        {
            _logger.LogError(
                "DailyLog not found for user {UserId} on date {Date}",
                notification.UserId,
                notification.DailyLogDate);
            return;  // ✅ Graceful exit instead of exception
        }

        var foodEntry = FoodEntry.CreateFromImageAnalyze(
            notification.FoodName,
            notification.QuantityInGrams,
            notification.TotalCalories,
            notification.TotalProteins,
            notification.TotalCarbohydrates,
            notification.TotalFats
        );

        // ✅ Use Snack meal instead of first meal
        var snackMeal = dailyLog.Meals.FirstOrDefault(m => m.MealType == MealType.Snack);
        if (snackMeal == null)
        {
            _logger.LogError("Snack meal not found in DailyLog for user {UserId}", notification.UserId);
            return;
        }

        snackMeal.AddFood(foodEntry);

        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation(
            "Successfully added food {FoodName} to Snack meal for user {UserId}",
            notification.FoodName,
            notification.UserId);
    }
}
```

**Verify:** Build the solution
```bash
dotnet build
```

---

**Task 1.2: Test the Complete Flow** (30 minutes)

1. **Start the application:**
```bash
dotnet run --project src/Bootstrapper/MealMind.Bootstrapper
```

2. **Sign up a new user** (via Postman/Swagger):
```http
POST /api/identity/sign-up
Content-Type: application/json

{
  "username": "testuser",
  "email": "test@example.com",
  "inputPassword": "Password123!",
  "personalData": {
    "gender": 0,
    "dateOfBirth": "1990-01-01",
    "weight": 75,
    "height": 180,
    "weightTarget": 70,
    "activityLevel": 3
  },
  "nutritionTargets": [
    {
      "calories": 2000,
      "nutritionInGramsPayload": {
        "proteinInGrams": 150,
        "carbohydratesInGrams": 200,
        "fatsInGrams": 65
      },
      "nutritionInPercentPayload": null,
      "waterIntake": 2.5,
      "activeDays": null
    }
  ]
}
```

3. **Sign in to get access token:**
```http
POST /api/identity/sign-in
Content-Type: application/json

{
  "email": "test@example.com",
  "password": "Password123!"
}
```

4. **Upload a food image:**
```http
POST /api/aichat/get-calories-from-image
Authorization: Bearer {your_access_token}
Content-Type: multipart/form-data

prompt: "Analyze this meal"
estimationMode: 1
dailyLogDate: 2025-11-26
saveFoodEntry: true
image: [upload a food photo]
```

5. **Verify food was added to daily log:**
```http
GET /api/nutrition/daily-log?date=2025-11-26
Authorization: Bearer {your_access_token}
```

**Expected Result:**
- You should see a food entry in the "Snack" meal
- The food name, calories, proteins, carbs, and fats should match the AI analysis

---

**Task 1.3: Check Logs** (10 minutes)

Look at your console output. You should see:
```
info: MealMind.Modules.Nutrition.Application.Events.Integration.ImageAnalyzeCreatedEventHandler[0]
      Processing ImageAnalyzeCreatedEvent for user {UserId}, food {FoodName}, date {Date}
info: MealMind.Modules.Nutrition.Application.Events.Integration.ImageAnalyzeCreatedEventHandler[0]
      Successfully processed ImageAnalyzeCreatedEvent for user {UserId}
```

If you see errors, debug and fix!

---

**Task 1.4: Commit Your Changes** (10 minutes)

```bash
git add .
git commit -m "Implement ImageAnalyzeCreatedEventHandler - complete image to food entry flow

- Process ImageAnalyzeCreatedEvent in Nutrition module
- Create FoodEntry from AI analysis data
- Add to Snack meal by default
- Add comprehensive logging for debugging

🤖 Generated with Claude Code"

git push origin master
```

---

**✅ Monday Done! You fixed the critical bug!** 🎉

---

### Tuesday: Background Job for DailyLog Maintenance

**Time:** 2-3 hours
**Difficulty:** ⭐⭐⭐☆☆ (Medium)

#### Tasks:

**Task 2.1: Create the Background Job** (1 hour)

**Create new file:** `src/Modules/Nutrition/MealMind.Modules.Nutrition.Infrastructure/BackgroundJobs/DailyLogMaintenanceJob.cs`

```csharp
using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MealMind.Modules.Nutrition.Infrastructure.BackgroundJobs;

public class DailyLogMaintenanceJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DailyLogMaintenanceJob> _logger;

    public DailyLogMaintenanceJob(
        IServiceProvider serviceProvider,
        ILogger<DailyLogMaintenanceJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DailyLogMaintenanceJob started");

        // Run daily at 2 AM
        using var timer = new PeriodicTimer(TimeSpan.FromHours(24));

        // Run immediately on startup (for testing)
        await ExtendDailyLogsForAllUsers(stoppingToken);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await ExtendDailyLogsForAllUsers(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DailyLogMaintenanceJob");
            }
        }
    }

    private async Task ExtendDailyLogsForAllUsers(CancellationToken ct)
    {
        _logger.LogInformation("Starting DailyLog extension for all users");

        using var scope = _serviceProvider.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var dailyLogRepository = scope.ServiceProvider.GetRequiredService<IDailyLogRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Get all users (paginate if you have many users)
        var users = await userRepository.GetAllAsync(ct);

        _logger.LogInformation("Found {Count} users to process", users.Count);

        var totalCreated = 0;

        foreach (var user in users)
        {
            try
            {
                var created = await ExtendDailyLogsForUser(user, dailyLogRepository, ct);
                totalCreated += created;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to extend DailyLogs for user {UserId}", user.Id);
                // Continue with other users
            }
        }

        await unitOfWork.CommitAsync(ct);

        _logger.LogInformation(
            "DailyLog extension completed. Created {TotalCreated} new DailyLogs for {UserCount} users",
            totalCreated,
            users.Count);
    }

    private async Task<int> ExtendDailyLogsForUser(
        Domain.UserProfile.UserProfile user,
        IDailyLogRepository dailyLogRepository,
        CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = today.AddDays(90); // Maintain 90-day window

        // Get existing dates for this user
        var existingDates = await dailyLogRepository.GetExistingDatesForUserAsync(
            user.Id,
            today,
            endDate,
            ct);

        // Find missing dates
        var allDates = Enumerable.Range(0, 90)
            .Select(i => today.AddDays(i))
            .ToHashSet();

        var missingDates = allDates.Except(existingDates).OrderBy(d => d).ToList();

        if (!missingDates.Any())
        {
            _logger.LogDebug("No missing DailyLogs for user {UserId}", user.Id);
            return 0;
        }

        _logger.LogInformation(
            "Creating {Count} missing DailyLogs for user {UserId}",
            missingDates.Count,
            user.Id);

        // Create missing DailyLogs
        foreach (var date in missingDates)
        {
            // Find the appropriate nutrition target for this day
            var target = user.NutritionTargets
                .FirstOrDefault(t => t.ActiveDays.Any(d => d.DayOfWeek == date.DayOfWeek))
                ?? user.NutritionTargets.FirstOrDefault();

            if (target is null)
            {
                _logger.LogWarning(
                    "No nutrition target found for user {UserId}. Skipping DailyLog creation.",
                    user.Id);
                continue;
            }

            var dailyLog = DailyLog.Create(date, null, target.Calories, user.Id);

            // Initialize all meal types
            foreach (var mealType in Enum.GetValues<MealType>())
            {
                var meal = Meal.Initialize(mealType, user.Id);
                dailyLog.AddMeal(meal);
            }

            await dailyLogRepository.AddAsync(dailyLog, ct);
        }

        return missingDates.Count;
    }
}
```

---

**Task 2.2: Add Repository Method** (20 minutes)

**Add to interface:** `src/Modules/Nutrition/MealMind.Modules.Nutrition.Application/Abstractions/Database/IDailyLogRepository.cs`

```csharp
Task<List<DateOnly>> GetExistingDatesForUserAsync(
    Guid userId,
    DateOnly startDate,
    DateOnly endDate,
    CancellationToken cancellationToken = default);
```

**Implement:** `src/Modules/Nutrition/MealMind.Modules.Nutrition.Infrastructure/Persistence/Repositories/DailyLogRepository.cs`

```csharp
public async Task<List<DateOnly>> GetExistingDatesForUserAsync(
    Guid userId,
    DateOnly startDate,
    DateOnly endDate,
    CancellationToken cancellationToken = default)
{
    return await _context.DailyLogs
        .Where(d => d.UserProfileId == userId && d.Date >= startDate && d.Date <= endDate)
        .Select(d => d.Date)
        .OrderBy(d => d)
        .ToListAsync(cancellationToken);
}
```

**Add to interface:** `src/Modules/Nutrition/MealMind.Modules.Nutrition.Application/Abstractions/Database/IUserProfileRepository.cs`

```csharp
Task<List<Domain.UserProfile.UserProfile>> GetAllAsync(CancellationToken cancellationToken = default);
```

**Implement:** `src/Modules/Nutrition/MealMind.Modules.Nutrition.Infrastructure/Persistence/Repositories/UserProfileRepository.cs`

```csharp
public async Task<List<Domain.UserProfile.UserProfile>> GetAllAsync(CancellationToken cancellationToken = default)
{
    return await _context.UserProfiles
        .Include(u => u.NutritionTargets)
            .ThenInclude(nt => nt.ActiveDays)
        .ToListAsync(cancellationToken);
}
```

---

**Task 2.3: Register the Background Job** (10 minutes)

**File:** `src/Modules/Nutrition/MealMind.Modules.Nutrition.Infrastructure/Extensions.cs`

Add at the end of the `AddInfrastructure` method:

```csharp
// Register background jobs
services.AddHostedService<BackgroundJobs.DailyLogMaintenanceJob>();
```

---

**Task 2.4: Test the Background Job** (30 minutes)

1. **Run the application:**
```bash
dotnet run --project src/Bootstrapper/MealMind.Bootstrapper
```

2. **Check logs on startup:**
You should see:
```
info: DailyLogMaintenanceJob[0]
      DailyLogMaintenanceJob started
info: DailyLogMaintenanceJob[0]
      Starting DailyLog extension for all users
info: DailyLogMaintenanceJob[0]
      Found {N} users to process
info: DailyLogMaintenanceJob[0]
      DailyLog extension completed. Created {N} new DailyLogs for {N} users
```

3. **Verify in database:**
```sql
-- Check if DailyLogs extend 90 days into future
SELECT
    up.username,
    COUNT(dl.id) as dailylog_count,
    MIN(dl.date) as first_date,
    MAX(dl.date) as last_date
FROM user_profiles up
LEFT JOIN daily_logs dl ON dl.user_profile_id = up.id
GROUP BY up.username;
```

Expected: Each user should have ~90 DailyLogs

4. **Test by deleting a future date:**
```sql
-- Delete a DailyLog 30 days in the future
DELETE FROM daily_logs
WHERE date = CURRENT_DATE + INTERVAL '30 days'
LIMIT 1;
```

5. **Restart the app and check if it gets recreated**

---

**Task 2.5: Commit Changes** (10 minutes)

```bash
git add .
git commit -m "Add DailyLogMaintenanceJob to extend daily logs automatically

- Background service runs daily at 2 AM
- Maintains 90-day window of DailyLogs for all users
- Handles missing dates gracefully
- Comprehensive logging for monitoring

🤖 Generated with Claude Code"

git push origin master
```

---

**✅ Tuesday Done! Background job is working!** 🎉

---

### Wednesday: Docker Setup (Part 1 - Dockerfile)

**Time:** 2-3 hours
**Difficulty:** ⭐⭐⭐☆☆ (Medium)

#### Tasks:

**Task 3.1: Create Production Dockerfile** (30 minutes)

**Create file:** `Dockerfile` (in repository root)

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY ["MealMind.sln", "./"]

# Copy all project files
COPY ["src/Bootstrapper/MealMind.Bootstrapper/MealMind.Bootstrapper.csproj", "src/Bootstrapper/MealMind.Bootstrapper/"]
COPY ["src/Shared/MealMind.Shared.Abstractions/MealMind.Shared.Abstractions.csproj", "src/Shared/MealMind.Shared.Abstractions/"]
COPY ["src/Shared/MealMind.Shared.Contracts/MealMind.Shared.Contracts.csproj", "src/Shared/MealMind.Shared.Contracts/"]
COPY ["src/Shared/MealMind.Shared.Infrastructure/MealMind.Shared.Infrastructure.csproj", "src/Shared/MealMind.Shared.Infrastructure/"]

# Identity module
COPY ["src/Modules/Identity/MealMind.Modules.Identity.Api/MealMind.Modules.Identity.Api.csproj", "src/Modules/Identity/MealMind.Modules.Identity.Api/"]
COPY ["src/Modules/Identity/MealMind.Modules.Identity.Application/MealMind.Modules.Identity.Application.csproj", "src/Modules/Identity/MealMind.Modules.Identity.Application/"]
COPY ["src/Modules/Identity/MealMind.Modules.Identity.Domain/MealMind.Modules.Identity.Domain.csproj", "src/Modules/Identity/MealMind.Modules.Identity.Domain/"]
COPY ["src/Modules/Identity/MealMind.Modules.Identity.Infrastructure/MealMind.Modules.Identity.Infrastructure.csproj", "src/Modules/Identity/MealMind.Modules.Identity.Infrastructure/"]

# Nutrition module
COPY ["src/Modules/Nutrition/MealMind.Modules.Nutrition.Api/MealMind.Modules.Nutrition.Api.csproj", "src/Modules/Nutrition/MealMind.Modules.Nutrition.Api/"]
COPY ["src/Modules/Nutrition/MealMind.Modules.Nutrition.Application/MealMind.Modules.Nutrition.Application.csproj", "src/Modules/Nutrition/MealMind.Modules.Nutrition.Application/"]
COPY ["src/Modules/Nutrition/MealMind.Modules.Nutrition.Domain/MealMind.Modules.Nutrition.Domain.csproj", "src/Modules/Nutrition/MealMind.Modules.Nutrition.Domain/"]
COPY ["src/Modules/Nutrition/MealMind.Modules.Nutrition.Infrastructure/MealMind.Modules.Nutrition.Infrastructure.csproj", "src/Modules/Nutrition/MealMind.Modules.Nutrition.Infrastructure/"]

# AiChat module
COPY ["src/Modules/AiChat/MealMind.Modules.AiChat.Api/MealMind.Modules.AiChat.Api.csproj", "src/Modules/AiChat/MealMind.Modules.AiChat.Api/"]
COPY ["src/Modules/AiChat/MealMind.Modules.AiChat.Application/MealMind.Modules.AiChat.Application.csproj", "src/Modules/AiChat/MealMind.Modules.AiChat.Application/"]
COPY ["src/Modules/AiChat/MealMind.Modules.AiChat.Domain/MealMind.Modules.AiChat.Domain.csproj", "src/Modules/AiChat/MealMind.Modules.AiChat.Domain/"]
COPY ["src/Modules/AiChat/MealMind.Modules.AiChat.Infrastructure/MealMind.Modules.AiChat.Infrastructure.csproj", "src/Modules/AiChat/MealMind.Modules.AiChat.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/Bootstrapper/MealMind.Bootstrapper/MealMind.Bootstrapper.csproj"

# Copy all source code
COPY . .

# Build
WORKDIR "/src/src/Bootstrapper/MealMind.Bootstrapper"
RUN dotnet build "MealMind.Bootstrapper.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "MealMind.Bootstrapper.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy published app
COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Set environment
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Run
ENTRYPOINT ["dotnet", "MealMind.Bootstrapper.dll"]
```

---

**Task 3.2: Create .dockerignore** (5 minutes)

**Create file:** `.dockerignore` (in repository root)

```
# Git
.git
.gitignore
.gitattributes

# Documentation
*.md
README.md
LICENSE

# IDE
.vs
.vscode
.idea
*.suo
*.user
*.userosscache
*.sln.docstates

# Build results
**/bin/
**/obj/
**/out/

# NuGet
*.nupkg
*.snupkg
packages/

# Test results
TestResults/
*.trx

# Docker
Dockerfile
.dockerignore
docker-compose*.yml

# Learning materials
LearningMaterials/

# Logs
*.log
logs/
```

---

**Task 3.3: Test Docker Build Locally** (1 hour)

1. **Build the image:**
```bash
docker build -t mealmind-api:test .
```

This will take 5-10 minutes the first time. Watch for errors!

2. **If build succeeds, run it:**
```bash
docker run -p 5000:8080 \
  -e DATABASE_CONNECTION_STRING="Host=host.docker.internal;Database=mealmind;Username=postgres;Password=yourpassword" \
  -e STRIPE_SECRET_KEY="your_stripe_key" \
  -e GEMINI_API_KEY="your_gemini_key" \
  mealmind-api:test
```

**Note:** `host.docker.internal` allows Docker to access your host machine's PostgreSQL

3. **Test the API:**
```bash
curl http://localhost:5000/health
# Should return: Healthy
```

4. **Check logs:**
Look for:
```
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: http://[::]:8080
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

5. **Test an endpoint:**
```bash
curl http://localhost:5000/api/identity/health
# Should return 200 OK
```

---

**Task 3.4: Fix Any Issues** (30 minutes)

**Common issues:**

**Issue 1: "Project file not found"**
- Check COPY paths in Dockerfile match your actual file structure
- Ensure all .csproj files are copied

**Issue 2: "Unable to connect to database"**
- Check `host.docker.internal` works (Windows/Mac)
- Linux users: Use `--network="host"` flag or actual IP

**Issue 3: Build takes forever**
- First build is slow (downloading SDK, packages)
- Subsequent builds use cache and are faster

---

**Task 3.5: Commit Docker Files** (10 minutes)

```bash
git add Dockerfile .dockerignore
git commit -m "Add production Dockerfile and .dockerignore

- Multi-stage build for optimized image size
- Health check endpoint configured
- Production-ready with environment variables
- Tested and working locally

🤖 Generated with Claude Code"

git push origin master
```

---

**✅ Wednesday Done! Docker image builds successfully!** 🎉

---

### Thursday: Docker Compose Setup

**Time:** 2-3 hours
**Difficulty:** ⭐⭐⭐☆☆ (Medium)

#### Tasks:

**Task 4.1: Create docker-compose.prod.yml** (30 minutes)

**Create file:** `docker-compose.prod.yml` (in repository root)

```yaml
version: '3.8'

services:
  # PostgreSQL Database with pgvector
  postgres:
    image: pgvector/pgvector:pg16
    container_name: mealmind-postgres
    environment:
      POSTGRES_DB: ${DB_NAME:-mealmind}
      POSTGRES_USER: ${DB_USER:-mealmind}
      POSTGRES_PASSWORD: ${DB_PASSWORD:?Database password required}
      POSTGRES_INITDB_ARGS: "--encoding=UTF8"
    volumes:
      - postgres-data:/var/lib/postgresql/data
    ports:
      - "5432:5432"
    restart: unless-stopped
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U $$POSTGRES_USER -d $$POSTGRES_DB"]
      interval: 10s
      timeout: 5s
      retries: 5

  # Ollama (AI Chat)
  ollama:
    image: ollama/ollama:latest
    container_name: mealmind-ollama
    volumes:
      - ollama-data:/root/.ollama
    ports:
      - "11434:11434"
    restart: unless-stopped
    environment:
      - OLLAMA_KEEP_ALIVE=24h
      - OLLAMA_NUM_PARALLEL=2

  # MealMind API
  api:
    build:
      context: .
      dockerfile: Dockerfile
    container_name: mealmind-api
    depends_on:
      postgres:
        condition: service_healthy
      ollama:
        condition: service_started
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:8080
      - DATABASE_CONNECTION_STRING=Host=postgres;Port=5432;Database=${DB_NAME:-mealmind};Username=${DB_USER:-mealmind};Password=${DB_PASSWORD};Include Error Detail=true
      - STRIPE_SECRET_KEY=${STRIPE_SECRET_KEY:?Stripe secret key required}
      - STRIPE_WEBHOOK_SECRET=${STRIPE_WEBHOOK_SECRET:?Stripe webhook secret required}
      - GEMINI_API_KEY=${GEMINI_API_KEY:?Gemini API key required}
      - JWT_SECRET=${JWT_SECRET:?JWT secret required}
      - OLLAMA_URL=http://ollama:11434
    ports:
      - "5000:8080"
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s

  # Caddy (Reverse Proxy + SSL)
  caddy:
    image: caddy:2-alpine
    container_name: mealmind-caddy
    depends_on:
      - api
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./Caddyfile:/etc/caddy/Caddyfile
      - caddy-data:/data
      - caddy-config:/config
    restart: unless-stopped

volumes:
  postgres-data:
    driver: local
  ollama-data:
    driver: local
  caddy-data:
    driver: local
  caddy-config:
    driver: local
```

---

**Task 4.2: Create Caddyfile** (15 minutes)

**Create file:** `Caddyfile` (in repository root)

```
# Development (local testing)
:80 {
    reverse_proxy api:8080
}

# Production (uncomment when you have a domain)
# api.mealmind.app {
#     reverse_proxy api:8080
#
#     # Logging
#     log {
#         output file /data/access.log
#     }
#
#     # Security headers
#     header {
#         # Enable HSTS
#         Strict-Transport-Security "max-age=31536000; includeSubDomains; preload"
#         # Prevent clickjacking
#         X-Frame-Options "SAMEORIGIN"
#         # Prevent MIME type sniffing
#         X-Content-Type-Options "nosniff"
#         # XSS Protection
#         X-XSS-Protection "1; mode=block"
#         # Referrer Policy
#         Referrer-Policy "strict-origin-when-cross-origin"
#     }
#
#     # Rate limiting (optional, requires Caddy plugin)
#     # rate_limit {
#     #     zone dynamic {
#     #         key {remote_host}
#     #         events 100
#     #         window 1m
#     #     }
#     # }
# }
```

---

**Task 4.3: Create Environment File Template** (10 minutes)

**Create file:** `.env.example` (in repository root)

```bash
# Database
DB_NAME=mealmind
DB_USER=mealmind
DB_PASSWORD=change_this_in_production

# Stripe
STRIPE_SECRET_KEY=sk_test_your_key_here
STRIPE_WEBHOOK_SECRET=whsec_your_webhook_secret_here

# AI Services
GEMINI_API_KEY=your_gemini_api_key_here

# JWT
JWT_SECRET=your_super_secret_jwt_key_minimum_32_chars_change_in_production

# Optional: If using cloud Ollama
# OLLAMA_URL=https://your-ollama-instance.com
```

**Create your actual `.env` file:**
```bash
cp .env.example .env
# Edit .env with your real values
```

**Add to .gitignore:**
```bash
echo ".env" >> .gitignore
```

---

**Task 4.4: Test Docker Compose Locally** (1 hour)

1. **Stop any running instances:**
```bash
# Stop local PostgreSQL if running
# Stop local Ollama if running
```

2. **Start the entire stack:**
```bash
docker-compose -f docker-compose.prod.yml up -d
```

This will:
- Pull PostgreSQL with pgvector
- Pull Ollama
- Build your API image
- Start all services

3. **Watch the logs:**
```bash
docker-compose -f docker-compose.prod.yml logs -f api
```

Look for:
```
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

4. **Check all services are healthy:**
```bash
docker-compose -f docker-compose.prod.yml ps
```

Should show all services as "Up (healthy)"

5. **Run database migrations:**
```bash
docker-compose -f docker-compose.prod.yml exec api dotnet ef database update --project /app/MealMind.Modules.Identity.Infrastructure.dll
docker-compose -f docker-compose.prod.yml exec api dotnet ef database update --project /app/MealMind.Modules.Nutrition.Infrastructure.dll
docker-compose -f docker-compose.prod.yml exec api dotnet ef database update --project /app/MealMind.Modules.AiChat.Infrastructure.dll
```

**OR** (better) create a migration script - see Task 4.5

6. **Test the API:**
```bash
# Health check
curl http://localhost/health

# Try to sign up
curl -X POST http://localhost/api/identity/sign-up \
  -H "Content-Type: application/json" \
  -d '{...}'
```

---

**Task 4.5: Create Migration Script** (20 minutes)

**Create file:** `scripts/migrate.sh`

```bash
#!/bin/bash
set -e

echo "🔄 Running database migrations..."

# Wait for database to be ready
echo "⏳ Waiting for database..."
until docker-compose -f docker-compose.prod.yml exec -T postgres pg_isready -U mealmind -d mealmind; do
  sleep 1
done

echo "✅ Database is ready!"

# Run migrations for each module
echo "📦 Migrating Identity module..."
docker-compose -f docker-compose.prod.yml exec -T api \
  dotnet ef database update \
  --project /app/MealMind.Modules.Identity.Infrastructure.dll \
  --startup-project /app/MealMind.Bootstrapper.dll \
  --context IdentityDbContext

echo "📦 Migrating Nutrition module..."
docker-compose -f docker-compose.prod.yml exec -T api \
  dotnet ef database update \
  --project /app/MealMind.Modules.Nutrition.Infrastructure.dll \
  --startup-project /app/MealMind.Bootstrapper.dll \
  --context NutritionDbContext

echo "📦 Migrating AiChat module..."
docker-compose -f docker-compose.prod.yml exec -T api \
  dotnet ef database update \
  --project /app/MealMind.Modules.AiChat.Infrastructure.dll \
  --startup-project /app/MealMind.Bootstrapper.dll \
  --context AiChatDbContext

echo "✅ All migrations completed successfully!"
```

Make it executable:
```bash
chmod +x scripts/migrate.sh
```

Run it:
```bash
./scripts/migrate.sh
```

---

**Task 4.6: Pull Ollama Models** (10 minutes)

```bash
# Pull models into Ollama container
docker-compose -f docker-compose.prod.yml exec ollama ollama pull llama3.2:3b
docker-compose -f docker-compose.prod.yml exec ollama ollama pull nomic-embed-text

# Verify
docker-compose -f docker-compose.prod.yml exec ollama ollama list
```

---

**Task 4.7: Commit Docker Compose Files** (10 minutes)

```bash
git add docker-compose.prod.yml Caddyfile .env.example .gitignore scripts/migrate.sh
git commit -m "Add Docker Compose production setup

- PostgreSQL with pgvector
- Ollama for AI chat
- API with health checks
- Caddy reverse proxy with auto-SSL
- Environment variable configuration
- Migration script for easy deployment

🤖 Generated with Claude Code"

git push origin master
```

---

**✅ Thursday Done! Full Docker stack working locally!** 🎉

---

### Friday: Prepare for VPS Deployment

**Time:** 2-3 hours
**Difficulty:** ⭐⭐⭐⭐☆ (Hard - but you got this!)

#### Tasks:

**Task 5.1: Buy a Domain** (30 minutes)

**Recommended registrars:**
- Namecheap (~$12/year)
- Google Domains → Squarespace (~$12/year)
- Cloudflare (~$10/year)

**Domain ideas:**
- `mealmind.app` (if available)
- `mealmind.io`
- `yourname-mealmind.com`

**After purchase:**
- Don't configure DNS yet (we'll do that Sunday)
- Just have the domain ready

---

**Task 5.2: Choose a VPS Provider** (30 minutes)

**Recommended options:**

| Provider | Cost | Specs | Pros |
|----------|------|-------|------|
| **DigitalOcean** | $6/month | 1GB RAM, 25GB SSD | Great docs, reliable |
| **Hetzner** | €4/month | 2GB RAM, 40GB SSD | Cheapest, powerful |
| **Linode** | $5/month | 1GB RAM, 25GB SSD | Good support |
| **Vultr** | $6/month | 1GB RAM, 25GB SSD | Fast network |

**My recommendation: DigitalOcean** for your first deployment (best docs)

**Sign up but don't create server yet** - we'll do that Saturday

---

**Task 5.3: Prepare Production Secrets** (30 minutes)

**Create a secure password manager entry** (use Bitwarden, 1Password, or similar):

```
=== MealMind Production Secrets ===

Database:
- DB_PASSWORD: [generate strong password - 32 chars]
- DB_USER: mealmind
- DB_NAME: mealmind

JWT:
- JWT_SECRET: [generate strong secret - 64 chars]

Stripe:
- STRIPE_SECRET_KEY: sk_live_... [from Stripe dashboard]
- STRIPE_WEBHOOK_SECRET: whsec_... [will generate on server]

Gemini:
- GEMINI_API_KEY: [your Gemini API key]

Server:
- SSH Key: [will generate Saturday]
- Root Password: [will set Saturday]
```

**Generate strong passwords:**
```bash
# On Linux/Mac
openssl rand -base64 32  # For DB_PASSWORD
openssl rand -hex 64     # For JWT_SECRET

# Or use online: https://passwordsgenerator.net/
```

---

**Task 5.4: Create Production Environment File** (15 minutes)

**Create file:** `.env.production` (DO NOT COMMIT THIS!)

```bash
# MealMind Production Environment
# ⚠️ NEVER commit this file to Git!

# Database
DB_NAME=mealmind
DB_USER=mealmind
DB_PASSWORD=your_super_secure_password_here

# Stripe (LIVE keys!)
STRIPE_SECRET_KEY=sk_live_your_live_key_here
STRIPE_WEBHOOK_SECRET=whsec_will_set_this_on_server

# Gemini AI
GEMINI_API_KEY=your_gemini_api_key_here

# JWT
JWT_SECRET=your_64_character_jwt_secret_here_make_it_very_long_and_random

# Domain (update after you set up DNS)
DOMAIN=api.mealmind.app
```

**Add to .gitignore:**
```bash
echo ".env.production" >> .gitignore
```

---

**Task 5.5: Create Deployment Documentation** (30 minutes)

**Create file:** `DEPLOYMENT.md` (in repository root)

```markdown
# MealMind Deployment Guide

## Prerequisites

- Domain name purchased
- VPS with Ubuntu 22.04 LTS
- SSH access to VPS
- Docker and Docker Compose installed on VPS

## Server Requirements

**Minimum:**
- 1GB RAM
- 25GB SSD
- 1 CPU core

**Recommended:**
- 2GB RAM
- 40GB SSD
- 2 CPU cores

## Deployment Steps

### 1. Initial Server Setup

```bash
# SSH into server
ssh root@your_server_ip

# Update system
apt update && apt upgrade -y

# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sh get-docker.sh

# Install Docker Compose
apt install docker-compose -y

# Create deployment user
adduser mealmind
usermod -aG docker mealmind
usermod -aG sudo mealmind

# Switch to deployment user
su - mealmind
```

### 2. Clone Repository

```bash
cd ~
git clone https://github.com/yourusername/MealMind.git
cd MealMind
```

### 3. Configure Environment

```bash
# Copy production environment file
cp .env.example .env

# Edit with production values
nano .env
```

### 4. Update Caddyfile

```bash
nano Caddyfile

# Update to:
# api.your-domain.com {
#     reverse_proxy api:8080
# }
```

### 5. Start Services

```bash
# Build and start
docker-compose -f docker-compose.prod.yml up -d

# Run migrations
./scripts/migrate.sh

# Pull Ollama models
docker-compose -f docker-compose.prod.yml exec ollama ollama pull llama3.2:3b
docker-compose -f docker-compose.prod.yml exec ollama ollama pull nomic-embed-text
```

### 6. Configure Firewall

```bash
# Allow SSH, HTTP, HTTPS
sudo ufw allow 22/tcp
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw enable
```

### 7. Point Domain to Server

In your domain registrar:
- Create A record: `api.your-domain.com` → Your server IP
- Wait for DNS propagation (5-30 minutes)

### 8. Verify Deployment

```bash
# Check all services running
docker-compose -f docker-compose.prod.yml ps

# Check API health
curl https://api.your-domain.com/health
```

## Updating the Application

```bash
cd ~/MealMind
git pull origin master
docker-compose -f docker-compose.prod.yml up -d --build
./scripts/migrate.sh
```

## Monitoring

```bash
# View logs
docker-compose -f docker-compose.prod.yml logs -f api

# Check resource usage
docker stats
```

## Backup Database

```bash
# Backup
docker-compose -f docker-compose.prod.yml exec postgres pg_dump -U mealmind mealmind > backup_$(date +%Y%m%d).sql

# Restore
docker-compose -f docker-compose.prod.yml exec -T postgres psql -U mealmind mealmind < backup_20250126.sql
```
```

---

**Task 5.6: Test Full Deployment Locally One More Time** (30 minutes)

**Complete end-to-end test:**

1. Stop all services:
```bash
docker-compose -f docker-compose.prod.yml down -v
```

2. Rebuild from scratch:
```bash
docker-compose -f docker-compose.prod.yml up -d --build
```

3. Run migrations:
```bash
./scripts/migrate.sh
```

4. Test signup → login → add food → image analysis

5. Verify background job:
```bash
docker-compose -f docker-compose.prod.yml logs api | grep DailyLogMaintenanceJob
```

---

**Task 5.7: Commit Final Changes** (10 minutes)

```bash
git add DEPLOYMENT.md
git commit -m "Add comprehensive deployment documentation

- Server setup instructions
- Docker deployment steps
- DNS configuration guide
- Monitoring and backup procedures
- Ready for production deployment

🤖 Generated with Claude Code"

git push origin master
```

---

**✅ Friday Done! Everything prepared for deployment!** 🎉

---

## 📊 Week 1 Progress Summary

**You accomplished:**
- ✅ Fixed ImageAnalyzeCreatedEventHandler (critical bug)
- ✅ Implemented background job for DailyLogs
- ✅ Created production Dockerfile
- ✅ Set up Docker Compose stack
- ✅ Prepared deployment documentation
- ✅ Tested everything locally

**Next Week:**
- Saturday: Create VPS and deploy
- Sunday: Configure domain and SSL
- Test from internet
- Celebrate! 🎉

---

## 🚀 Saturday & Sunday Preview

### Saturday Tasks (3-4 hours):
1. Create DigitalOcean droplet
2. SSH setup
3. Install Docker
4. Clone repo
5. Deploy with docker-compose
6. Configure Stripe webhooks

### Sunday Tasks (1-2 hours):
1. Point domain to server
2. Update Caddyfile with domain
3. Test SSL certificate
4. Test API from internet
5. **CELEBRATE! YOUR API IS LIVE!** 🎉

---

## 💡 Tips for Success This Week

**Monday-Tuesday:**
- Take breaks between tasks
- Test thoroughly before moving on
- Commit after each task

**Wednesday-Thursday:**
- Docker can be tricky - don't rush
- Read error messages carefully
- Google any issues (Docker has great community)

**Friday:**
- This is prep day - no coding
- Get your accounts ready
- Review documentation

**Weekend:**
- Block out 4-5 hours
- Have your laptop charged
- Have coffee/tea ready ☕
- This is the exciting part!

---

## 📞 Getting Help

**If you get stuck:**

1. **Check logs first:**
```bash
docker-compose -f docker-compose.prod.yml logs api
```

2. **Google the error** - Docker errors are well-documented

3. **Common issues:**
   - Port conflicts: Stop other services
   - Permission errors: Use `sudo` or add user to docker group
   - Out of disk space: Clean up with `docker system prune`

4. **Ask Claude Code!** - Share error messages and I can help debug

---

## 🎯 Success Criteria

**By end of Week 1, you should have:**
- [ ] ImageAnalyzeCreatedEventHandler working
- [ ] Background job extending DailyLogs
- [ ] Docker image building successfully
- [ ] Full stack running in Docker Compose
- [ ] Domain purchased
- [ ] VPS account created
- [ ] All documentation ready

**If you have all checkmarks: You're ready to deploy! 🚀**

---

## 🎉 Motivation

Remember why you're doing this:
- 💪 You're building something **real**
- 🚀 You're **10 days away** from having a live API
- 📈 Every task gets you closer to **real users**
- 🎓 You're learning **production-grade** skills
- 💼 This is an **impressive portfolio project**

**You've already built the hard parts. Now you're just deploying it!**

Let's do this! 💪🔥

---

**Ready to start?**

Begin with **Monday - Task 1.1** and work your way through!

Good luck! 🚀
