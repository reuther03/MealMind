# TUnit Testing Framework - Quick Guide

## Overview
TUnit is a modern, fast, and powerful testing framework for .NET that serves as an alternative to xUnit, NUnit, and MSTest. It's designed with performance and developer experience in mind.

## Installation
```bash
dotnet add package TUnit
dotnet add package TUnit.Assertions
```

## Basic Test Structure

### Simple Test
```csharp
using TUnit.Core;
using TUnit.Assertions;

public class CalculatorTests
{
    [Test]
    public async Task Add_TwoNumbers_ReturnsSum()
    {
        // Arrange
        var calculator = new Calculator();
        
        // Act
        var result = calculator.Add(2, 3);
        
        // Assert
        await Assert.That(result).IsEqualTo(5);
    }
}
```

## Key Attributes

### Test Attributes
```csharp
[Test] // Basic test
[Test(Skip = "Not implemented yet")] // Skip test
[Test(Timeout = 1000)] // Timeout in milliseconds
[NotInParallel] // Run sequentially
[Repeat(3)] // Run test 3 times
```

### Test Lifecycle
```csharp
public class MyTests
{
    [Before(Test)]
    public async Task Setup()
    {
        // Runs before each test
    }

    [After(Test)]
    public async Task Cleanup()
    {
        // Runs after each test
    }

    [Before(Class)]
    public static async Task ClassSetup()
    {
        // Runs once before all tests in class
    }

    [After(Class)]
    public static async Task ClassCleanup()
    {
        // Runs once after all tests in class
    }
}
```

## Data-Driven Tests

### Using Arguments
```csharp
[Test]
[Arguments(1, 2, 3)]
[Arguments(5, 5, 10)]
[Arguments(-1, 1, 0)]
public async Task Add_MultipleInputs_ReturnsCorrectSum(int a, int b, int expected)
{
    var result = calculator.Add(a, b);
    await Assert.That(result).IsEqualTo(expected);
}
```

### Using MethodData
```csharp
[Test]
[MethodDataSource(nameof(GetTestData))]
public async Task TestWithMethodData(int value, bool expected)
{
    var result = IsEven(value);
    await Assert.That(result).IsEqualTo(expected);
}

public static IEnumerable<(int, bool)> GetTestData()
{
    yield return (2, true);
    yield return (3, false);
    yield return (4, true);
}
```

## Assertions

### Basic Assertions
```csharp
// Equality
await Assert.That(actual).IsEqualTo(expected);
await Assert.That(actual).IsNotEqualTo(notExpected);

// Null checks
await Assert.That(value).IsNull();
await Assert.That(value).IsNotNull();

// Boolean
await Assert.That(condition).IsTrue();
await Assert.That(condition).IsFalse();

// Comparisons
await Assert.That(value).IsGreaterThan(5);
await Assert.That(value).IsLessThan(10);
await Assert.That(value).IsGreaterThanOrEqualTo(5);

// Strings
await Assert.That(text).Contains("substring");
await Assert.That(text).StartsWith("prefix");
await Assert.That(text).EndsWith("suffix");
await Assert.That(text).Matches(@"\d{3}-\d{3}-\d{4}"); // Regex

// Collections
await Assert.That(list).Contains(item);
await Assert.That(list).HasCount(5);
await Assert.That(list).IsEmpty();
await Assert.That(list).IsNotEmpty();
```

### Exception Assertions
```csharp
[Test]
public async Task Method_InvalidInput_ThrowsException()
{
    await Assert.ThrowsAsync<ArgumentException>(() => 
        service.ProcessAsync(null)
    );
}

// With exception validation
[Test]
public async Task Method_ThrowsWithSpecificMessage()
{
    var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
        service.DoSomethingAsync()
    );
    
    await Assert.That(exception.Message).Contains("specific message");
}
```

## Dependency Injection

### Using Test Services
```csharp
public class ServiceTests
{
    private readonly IMyService _service;
    
    public ServiceTests()
    {
        var services = new ServiceCollection();
        services.AddScoped<IMyService, MyService>();
        services.AddScoped<IRepository, MockRepository>();
        
        var provider = services.BuildServiceProvider();
        _service = provider.GetRequiredService<IMyService>();
    }
    
    [Test]
    public async Task Service_PerformsAction_Successfully()
    {
        var result = await _service.DoActionAsync();
        await Assert.That(result).IsNotNull();
    }
}
```

## Testing Async Code

```csharp
[Test]
public async Task AsyncMethod_ReturnsExpectedValue()
{
    // TUnit is async-first, all tests can be async
    var result = await service.GetDataAsync();
    
    await Assert.That(result).IsNotNull();
    await Assert.That(result.Count).IsEqualTo(5);
}

[Test]
[Timeout(5000)] // 5 seconds timeout
public async Task LongRunningOperation_CompletesInTime()
{
    await service.LongOperationAsync();
}
```

## Parallel Execution

```csharp
// Tests run in parallel by default

[NotInParallel] // Force sequential execution
public class DatabaseTests
{
    [Test]
    public async Task Test1() { }
    
    [Test]
    public async Task Test2() { }
}

// Or control at test level
[Test]
[NotInParallel("Database")] // Named exclusion group
public async Task DatabaseTest1() { }

[Test]
[NotInParallel("Database")]
public async Task DatabaseTest2() { }
```

## Test Output

```csharp
public class TestWithOutput
{
    private readonly ITestOutputHelper _output;
    
    public TestWithOutput(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Test]
    public async Task TestWithLogging()
    {
        _output.WriteLine("Starting test...");
        
        var result = await ProcessData();
        
        _output.WriteLine($"Result: {result}");
        
        await Assert.That(result).IsNotNull();
    }
}
```

## Best Practices

### 1. Test Naming
```csharp
[Test]
public async Task MethodName_StateUnderTest_ExpectedBehavior()
{
    // Example: Add_TwoPositiveNumbers_ReturnsSum
}
```

### 2. AAA Pattern
```csharp
[Test]
public async Task ProperTestStructure()
{
    // Arrange - Set up test data
    var service = new Service();
    var input = new TestData();
    
    // Act - Execute the action
    var result = await service.ProcessAsync(input);
    
    // Assert - Verify the outcome
    await Assert.That(result.Success).IsTrue();
}
```

### 3. One Assertion Per Test (When Possible)
```csharp
// Good - Focused test
[Test]
public async Task User_Creation_SetsCorrectUsername()
{
    var user = new User("john_doe");
    await Assert.That(user.Username).IsEqualTo("john_doe");
}

// Also acceptable - Related assertions
[Test]
public async Task User_Creation_SetsAllProperties()
{
    var user = new User("john_doe", "john@example.com");
    
    await Assert.Multiple(() =>
    {
        Assert.That(user.Username).IsEqualTo("john_doe");
        Assert.That(user.Email).IsEqualTo("john@example.com");
        Assert.That(user.CreatedAt).IsNotNull();
    });
}
```

## Running Tests

### Command Line
```bash
# Run all tests
dotnet test

# Run with verbosity
dotnet test -v normal

# Run specific test class
dotnet test --filter "FullyQualifiedName~CalculatorTests"

# Run tests matching pattern
dotnet test --filter "Name~Add"
```

### VS Code / Visual Studio
- Tests appear in Test Explorer
- Can run/debug individual tests or groups
- Live test execution during development

## Advantages of TUnit

1. **Performance**: Faster test execution than traditional frameworks
2. **Modern API**: Async-first, cleaner syntax
3. **Better Assertions**: Fluent, chainable assertions
4. **Parallel by Default**: Tests run in parallel unless specified
5. **Source Generators**: Compile-time test discovery
6. **Minimal Boilerplate**: Less setup code needed

## Example: Testing a Repository

```csharp
public class UserRepositoryTests
{
    private readonly UserRepository _repository;
    private readonly TestDbContext _context;
    
    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;
            
        _context = new TestDbContext(options);
        _repository = new UserRepository(_context);
    }
    
    [After(Test)]
    public async Task Cleanup()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }
    
    [Test]
    public async Task AddUser_ValidUser_SavesSuccessfully()
    {
        // Arrange
        var user = new User("testuser", "test@example.com");
        
        // Act
        await _repository.AddAsync(user);
        await _context.SaveChangesAsync();
        
        // Assert
        var savedUser = await _repository.GetByIdAsync(user.Id);
        await Assert.That(savedUser).IsNotNull();
        await Assert.That(savedUser.Username).IsEqualTo("testuser");
    }
    
    [Test]
    public async Task GetByEmail_ExistingEmail_ReturnsUser()
    {
        // Arrange
        var user = new User("testuser", "test@example.com");
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        
        // Act
        var result = await _repository.GetByEmailAsync("test@example.com");
        
        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Id).IsEqualTo(user.Id);
    }
    
    [Test]
    public async Task Delete_ExistingUser_RemovesFromDatabase()
    {
        // Arrange
        var user = new User("testuser", "test@example.com");
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        
        // Act
        await _repository.DeleteAsync(user);
        await _context.SaveChangesAsync();
        
        // Assert
        var deletedUser = await _repository.GetByIdAsync(user.Id);
        await Assert.That(deletedUser).IsNull();
    }
}
```

## Migration from xUnit/NUnit

### xUnit → TUnit
```csharp
// xUnit
[Fact] → [Test]
[Theory] → [Test]
[InlineData] → [Arguments]
Assert.Equal(expected, actual) → await Assert.That(actual).IsEqualTo(expected)
Assert.Null(value) → await Assert.That(value).IsNull()
Assert.Throws<T>() → await Assert.ThrowsAsync<T>()
```

### NUnit → TUnit
```csharp
// NUnit  
[Test] → [Test]
[TestCase] → [Arguments]
[SetUp] → [Before(Test)]
[TearDown] → [After(Test)]
Assert.AreEqual(expected, actual) → await Assert.That(actual).IsEqualTo(expected)
Assert.That(actual, Is.EqualTo(expected)) → await Assert.That(actual).IsEqualTo(expected)
```

## Resources

- GitHub: https://github.com/thomhurst/TUnit
- Documentation: https://thomhurst.github.io/TUnit/
- NuGet: https://www.nuget.org/packages/TUnit