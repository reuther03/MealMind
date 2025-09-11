# Types of Testing - Complete Guide

## Testing Pyramid Overview

```
          /\
         /E2E\        <- End-to-End Tests (Few)
        /------\
       /  Integ \     <- Integration Tests (Some)
      /----------\
     /    Unit    \   <- Unit Tests (Many)
    /--------------\
```

The testing pyramid shows the ideal distribution of test types - many fast unit tests at the base, fewer integration tests in the middle, and minimal E2E tests at the top.

## 1. Unit Tests

### What They Are
Unit tests verify the smallest testable parts of code in isolation - typically individual methods or classes. External dependencies are mocked or stubbed.

### Characteristics
- **Fast**: Milliseconds to run
- **Isolated**: No database, file system, or network calls
- **Deterministic**: Same input always produces same output
- **Focused**: Test one thing at a time

### What to Test
- Business logic
- Algorithms
- Value objects
- Domain entities
- Utility functions
- Edge cases and error handling

### Example - Testing a Domain Entity
```csharp
public class NutritionTargetTests
{
    [Test]
    public async Task Create_ValidInputs_CreatesTarget()
    {
        // Arrange
        var calories = 2000m;
        var protein = 150m;
        var userId = Guid.NewGuid();
        
        // Act
        var target = NutritionTarget.Create(calories, protein, 250m, 65m, 2.5m, userId);
        
        // Assert
        await Assert.That(target.Calories).IsEqualTo(calories);
        await Assert.That(target.Protein).IsEqualTo(protein);
        await Assert.That(target.IsActive).IsTrue();
    }
    
    [Test]
    public async Task Deactivate_ActiveTarget_SetsInactiveAndDate()
    {
        // Arrange
        var target = NutritionTarget.Create(2000m, 150m, 250m, 65m, 2.5m, Guid.NewGuid());
        
        // Act
        target.Deactivate();
        
        // Assert
        await Assert.That(target.IsActive).IsFalse();
        await Assert.That(target.DeactivatedAt).IsNotNull();
    }
}
```

### Example - Testing a Service with Mocks
```csharp
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly UserService _service;
    
    public UserServiceTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _service = new UserService(_repositoryMock.Object);
    }
    
    [Test]
    public async Task GetUser_ExistingId_ReturnsUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedUser = new User("john", "john@example.com");
        _repositoryMock.Setup(r => r.GetByIdAsync(userId))
                       .ReturnsAsync(expectedUser);
        
        // Act
        var result = await _service.GetUserAsync(userId);
        
        // Assert
        await Assert.That(result).IsEqualTo(expectedUser);
        _repositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
    }
}
```

## 2. Integration Tests

### What They Are
Integration tests verify that different parts of the system work together correctly. They test the interaction between multiple components.

### Characteristics
- **Slower than unit tests**: Seconds to run
- **Use real implementations**: Real database, but maybe in-memory
- **Test boundaries**: API endpoints, database operations
- **May use test doubles**: For external services

### What to Test
- Database operations (repositories)
- API endpoints
- Command/Query handlers with real database
- Multiple layers working together
- Transaction boundaries

### Example - Testing a Repository with Real Database
```csharp
public class UserRepositoryIntegrationTests : IDisposable
{
    private readonly NutritionDbContext _context;
    private readonly UserRepository _repository;
    
    public UserRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<NutritionDbContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;
            
        _context = new NutritionDbContext(options);
        _repository = new UserRepository(_context);
    }
    
    [Test]
    public async Task AddAndRetrieve_User_WorksEndToEnd()
    {
        // Arrange
        var user = User.Create("testuser", "test@example.com");
        
        // Act - Add user
        await _repository.AddAsync(user);
        await _context.SaveChangesAsync();
        
        // Act - Retrieve user
        var retrieved = await _repository.GetByEmailAsync("test@example.com");
        
        // Assert
        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved.Id).IsEqualTo(user.Id);
        await Assert.That(retrieved.Email.Value).IsEqualTo("test@example.com");
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }
}
```

### Example - Testing Command Handler
```csharp
public class AddNutritionTargetCommandTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ISender _mediator;
    private readonly NutritionDbContext _context;
    
    public AddNutritionTargetCommandTests()
    {
        var services = new ServiceCollection();
        
        // Add real implementations
        services.AddDbContext<NutritionDbContext>(options =>
            options.UseInMemoryDatabase($"Test_{Guid.NewGuid()}"));
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddNutritionTargetCommand).Assembly));
        
        _serviceProvider = services.BuildServiceProvider();
        _mediator = _serviceProvider.GetRequiredService<ISender>();
        _context = _serviceProvider.GetRequiredService<NutritionDbContext>();
    }
    
    [Test]
    public async Task Handle_ValidCommand_CreatesNutritionTarget()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userProfile = UserProfile.Create(userId, "testuser", "test@example.com");
        await _context.UserProfiles.AddAsync(userProfile);
        await _context.SaveChangesAsync();
        
        var command = new AddNutritionTargetCommand(
            UserId: userId,
            Calories: 2000,
            Protein: 150,
            Carbohydrates: 250,
            Fats: 65,
            WaterIntake: 2.5
        );
        
        // Act
        var result = await _mediator.Send(command);
        
        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        
        var savedProfile = await _context.UserProfiles
            .Include(p => p.NutritionTargets)
            .FirstAsync(p => p.Id == userId);
            
        await Assert.That(savedProfile.NutritionTargets).HasCount(1);
        await Assert.That(savedProfile.NutritionTargets[0].Calories).IsEqualTo(2000);
    }
}
```

## 3. End-to-End (E2E) Tests

### What They Are
E2E tests verify complete user scenarios from start to finish, testing the entire application stack as users would interact with it.

### Characteristics
- **Slowest**: Seconds to minutes
- **Full stack**: Real database, real API, sometimes real UI
- **User perspective**: Test user journeys
- **Brittle**: Can break due to UI or flow changes
- **Expensive**: Take time to write and maintain

### What to Test
- Critical user journeys
- Happy path scenarios
- Complete workflows
- Integration with external services

### Example - API End-to-End Test
```csharp
public class NutritionApiE2ETests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    
    public NutritionApiE2ETests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Override with test database
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<NutritionDbContext>));
                services.Remove(descriptor);
                
                services.AddDbContext<NutritionDbContext>(options =>
                    options.UseInMemoryDatabase("E2ETest"));
            });
        });
        
        _client = _factory.CreateClient();
    }
    
    [Test]
    public async Task CompleteUserJourney_CreateProfileAndAddTarget()
    {
        // Step 1: Sign up user
        var signUpRequest = new
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Test123!"
        };
        
        var signUpResponse = await _client.PostAsJsonAsync("/sign-up", signUpRequest);
        await Assert.That(signUpResponse.StatusCode).IsEqualTo(HttpStatusCode.OK);
        var userId = await signUpResponse.Content.ReadFromJsonAsync<Guid>();
        
        // Step 2: Set personal data
        var personalDataRequest = new
        {
            UserId = userId,
            Gender = "Male",
            DateOfBirth = "1990-01-01",
            Weight = 75,
            Height = 180,
            ActivityLevel = "Moderate"
        };
        
        var personalDataResponse = await _client.PostAsJsonAsync(
            "/nutrition-module/personal-data", personalDataRequest);
        await Assert.That(personalDataResponse.StatusCode).IsEqualTo(HttpStatusCode.OK);
        
        // Step 3: Add nutrition target
        var nutritionTargetRequest = new
        {
            UserId = userId,
            Calories = 2500,
            Protein = 180,
            Carbohydrates = 300,
            Fats = 80,
            WaterIntake = 3.0
        };
        
        var targetResponse = await _client.PostAsJsonAsync(
            "/nutrition-module/nutrition-target", nutritionTargetRequest);
        await Assert.That(targetResponse.StatusCode).IsEqualTo(HttpStatusCode.OK);
        
        // Step 4: Verify complete profile
        var profileResponse = await _client.GetAsync($"/nutrition-module/profile/{userId}");
        await Assert.That(profileResponse.StatusCode).IsEqualTo(HttpStatusCode.OK);
        
        var profile = await profileResponse.Content.ReadFromJsonAsync<UserProfileDto>();
        await Assert.That(profile.PersonalData).IsNotNull();
        await Assert.That(profile.NutritionTargets).HasCount(1);
    }
}
```

## 4. Domain Tests

### What They Are
Domain tests focus specifically on domain logic, ensuring business rules and invariants are maintained. They're a subset of unit tests focused on the domain layer.

### Characteristics
- **Pure business logic**: No infrastructure concerns
- **Test invariants**: Ensure domain rules are enforced
- **Value objects and entities**: Core domain concepts
- **Domain services**: Complex business operations

### Example - Domain Aggregate Tests
```csharp
public class WeekLogDomainTests
{
    [Test]
    public async Task WeekLog_CannotAddEntryOutsideWeekRange()
    {
        // Arrange
        var weekLog = WeekLog.CreateForDate(new DateOnly(2025, 1, 6), Guid.NewGuid()); // Monday
        var outsideDate = new DateOnly(2025, 1, 15); // Next week
        
        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() =>
            weekLog.AddFoodEntry(outsideDate, new FoodEntry(...))
        );
    }
    
    [Test]
    public async Task Food_NutritionCalculation_MaintainsAccuracy()
    {
        // Arrange
        var food = Food.Create(
            name: "Chicken Breast",
            nutrition: new NutritionPer100g(165, 31, 3.6m, 0)
        );
        
        // Act
        var nutrition150g = food.CalculateNutritionForGrams(150);
        
        // Assert
        await Assert.That(nutrition150g.Calories).IsEqualTo(247.5m); // 165 * 1.5
        await Assert.That(nutrition150g.Protein).IsEqualTo(46.5m);   // 31 * 1.5
    }
}
```

## 5. Acceptance Tests

### What They Are
Acceptance tests verify that the system meets business requirements and acceptance criteria. Often written in business-readable language.

### Example - Using SpecFlow/Gherkin
```gherkin
Feature: Nutrition Tracking
    As a user
    I want to track my daily nutrition
    So that I can meet my health goals
    
Scenario: Track daily calorie intake
    Given I have a nutrition target of 2000 calories
    When I log the following foods:
        | Food           | Quantity | Calories |
        | Chicken Breast | 200g     | 330      |
        | Brown Rice     | 150g     | 168      |
        | Broccoli       | 100g     | 34       |
    Then my total calorie intake should be 532
    And I should have 1468 calories remaining
```

## 6. Performance Tests

### What They Are
Performance tests measure system performance under various conditions.

### Example
```csharp
[Test]
[Benchmark]
public async Task GetUserProfile_LoadTest()
{
    var tasks = new List<Task>();
    
    // Simulate 100 concurrent requests
    for (int i = 0; i < 100; i++)
    {
        tasks.Add(_client.GetAsync($"/profile/{_testUserId}"));
    }
    
    var stopwatch = Stopwatch.StartNew();
    await Task.WhenAll(tasks);
    stopwatch.Stop();
    
    await Assert.That(stopwatch.ElapsedMilliseconds).IsLessThan(5000);
}
```

## Testing Strategy for Your MealMind Project

### 1. Unit Tests (70% of tests)
Focus on:
- Domain entities (Food, WeekLog, DailyLog, NutritionTarget)
- Value objects (NutritionPer100g, PersonalData)
- Domain services
- Business rule validation

### 2. Integration Tests (20% of tests)
Focus on:
- Repository operations
- Command/Query handlers
- Database transactions
- Module interactions

### 3. E2E Tests (10% of tests)
Focus on:
- Complete user registration flow
- Weekly nutrition tracking workflow
- Food search and logging journey

### Test Organization Structure
```
tests/
├── MealMind.Modules.Nutrition.Domain.Tests/       # Domain unit tests
│   ├── Food/
│   │   ├── FoodTests.cs
│   │   └── NutritionPer100gTests.cs
│   └── UserProfile/
│       ├── NutritionTargetTests.cs
│       └── UserProfileTests.cs
├── MealMind.Modules.Nutrition.Application.Tests/  # Application tests
│   ├── Commands/
│   │   ├── AddNutritionTargetCommandTests.cs
│   │   └── SetPersonalDataCommandTests.cs
│   └── Queries/
│       └── GetUserProfileQueryTests.cs
├── MealMind.Modules.Nutrition.Integration.Tests/  # Integration tests
│   ├── Repositories/
│   │   └── UserProfileRepositoryTests.cs
│   └── ExternalServices/
│       └── OpenFoodFactsServiceTests.cs
└── MealMind.E2E.Tests/                           # End-to-end tests
    ├── NutritionModuleE2ETests.cs
    └── IdentityModuleE2ETests.cs
```

## When to Use Each Type

### Unit Tests
✅ Use when:
- Testing business logic
- Testing algorithms
- Testing edge cases
- Need fast feedback
- Testing in isolation

❌ Don't use when:
- Testing integration points
- Testing database operations
- Testing external service calls

### Integration Tests
✅ Use when:
- Testing database operations
- Testing API endpoints
- Testing service interactions
- Verifying transaction boundaries

❌ Don't use when:
- Testing pure business logic
- Need very fast tests
- Testing UI interactions

### E2E Tests
✅ Use when:
- Testing critical user paths
- Testing complete workflows
- Final validation before release
- Testing third-party integrations

❌ Don't use when:
- Testing edge cases
- Need fast feedback
- Testing individual components

## Best Practices

### 1. Test Naming Convention
```csharp
[MethodName]_[Scenario]_[ExpectedResult]

Examples:
- Create_ValidInputs_ReturnsNewEntity
- AddFood_DateOutsideWeekRange_ThrowsException
- GetProfile_NonExistentUser_ReturnsNull
```

### 2. Test Data Builders
```csharp
public class UserProfileBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _username = "testuser";
    private string _email = "test@example.com";
    
    public UserProfileBuilder WithUsername(string username)
    {
        _username = username;
        return this;
    }
    
    public UserProfileBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }
    
    public UserProfile Build()
    {
        return UserProfile.Create(_id, _username, _email);
    }
}

// Usage
var profile = new UserProfileBuilder()
    .WithUsername("john")
    .WithEmail("john@example.com")
    .Build();
```

### 3. Test Isolation
- Each test should be independent
- Use fresh database for each test
- Clean up after tests
- Don't depend on test execution order

### 4. Assertion Messages
```csharp
await Assert.That(result.IsSuccess)
    .WithMessage("Command should succeed with valid input")
    .IsTrue();
```

### 5. Avoid Logic in Tests
```csharp
// Bad - logic in test
[Test]
public async Task BadTest()
{
    var value = 10;
    if (value > 5)
    {
        await Assert.That(Calculate(value)).IsEqualTo(20);
    }
}

// Good - explicit test
[Test]
public async Task Calculate_ValueGreaterThan5_ReturnsDouble()
{
    var result = Calculate(10);
    await Assert.That(result).IsEqualTo(20);
}
```

## Common Testing Patterns

### 1. Arrange-Act-Assert (AAA)
```csharp
[Test]
public async Task StandardTestStructure()
{
    // Arrange - Setup
    var service = new Service();
    var input = new Input();
    
    // Act - Execute
    var result = await service.Process(input);
    
    // Assert - Verify
    await Assert.That(result).IsNotNull();
}
```

### 2. Given-When-Then (BDD)
```csharp
[Test]
public async Task BddStyleTest()
{
    // Given - Context
    var user = GivenAUserWithNutritionTarget(2000);
    
    // When - Action
    var remaining = WhenCalculatingRemainingCalories(user, consumed: 500);
    
    // Then - Outcome
    await ThenRemainingShouldBe(remaining, expected: 1500);
}
```

### 3. Test Fixtures
```csharp
public class DatabaseFixture : IDisposable
{
    public NutritionDbContext Context { get; }
    
    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<NutritionDbContext>()
            .UseInMemoryDatabase($"Test_{Guid.NewGuid()}")
            .Options;
        Context = new NutritionDbContext(options);
    }
    
    public void Dispose() => Context.Dispose();
}
```

## Red-Green-Refactor Cycle (TDD)

1. **Red**: Write a failing test
2. **Green**: Write minimal code to pass
3. **Refactor**: Improve code while keeping tests green

```csharp
// Step 1: Red - Write failing test
[Test]
public async Task Food_CalculateCalories_ForGrams()
{
    var food = new Food("Apple", new NutritionPer100g(52, 0.3m, 0.2m, 14));
    var calories = food.CalculateCaloriesForGrams(150);
    await Assert.That(calories).IsEqualTo(78);
}

// Step 2: Green - Make it pass
public decimal CalculateCaloriesForGrams(decimal grams)
{
    return Nutrition.Calories * (grams / 100);
}

// Step 3: Refactor - Improve
public decimal CalculateCaloriesForGrams(decimal grams)
{
    const decimal GRAMS_PER_HUNDRED = 100m;
    return Nutrition.Calories * (grams / GRAMS_PER_HUNDRED);
}
```

## Testing Checklist

- [ ] Unit tests for all domain entities
- [ ] Unit tests for value objects
- [ ] Integration tests for repositories
- [ ] Integration tests for command/query handlers
- [ ] E2E tests for critical user paths
- [ ] Tests for edge cases
- [ ] Tests for error scenarios
- [ ] Performance tests for critical operations
- [ ] All tests are independent
- [ ] Tests run in CI/CD pipeline