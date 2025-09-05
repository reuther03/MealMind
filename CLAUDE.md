# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview
MealMind is a modular .NET 9 application built with clean architecture principles. It manages nutrition and user identity features with a focus on domain-driven design.

## Architecture

### Solution Structure
- **Bootstrapper**: Main entry point that orchestrates module loading and dependency injection
- **Modules**: Feature-specific implementations following vertical slice architecture
  - Identity: User authentication and management
  - Nutrition: User profiles and nutrition tracking
- **Shared**: Cross-cutting concerns and abstractions

### Key Patterns
- **Modular Monolith**: Each module (Identity, Nutrition) is self-contained with Api, Application, Domain, and Infrastructure layers
- **Module System**: Modules implement `IModule` interface and are dynamically loaded at startup
- **Repository Pattern**: Data access through domain-specific repositories implementing `IRepository<T>`
- **Value Objects**: Domain modeling uses value objects (e.g., PersonalData, NutritionTarget)

## Commands

### Development
```bash
# Build the solution
dotnet build

# Run the application
dotnet run --project src/Bootstrapper/MealMind.Bootstrapper

# Apply database migrations
dotnet ef database update --project src/Modules/Nutrition/MealMind.Modules.Nutrition.Infrastructure --startup-project src/Bootstrapper/MealMind.Bootstrapper

# Add new migration (example for Nutrition module)
dotnet ef migrations add MigrationName --project src/Modules/Nutrition/MealMind.Modules.Nutrition.Infrastructure --startup-project src/Bootstrapper/MealMind.Bootstrapper
```

### Docker
```bash
# Start PostgreSQL database
docker-compose up -d

# Stop services
docker-compose down
```

## Database
- PostgreSQL on port 5433 (configured in docker-compose.yml)
- Connection string in appsettings.json
- Each module manages its own DbContext and migrations

## Module Development
When adding features to a module:
1. Domain entities go in `Modules/[ModuleName]/Domain`
2. Application logic and use cases in `Modules/[ModuleName]/Application`
3. API endpoints extend `EndpointBase` in `Modules/[ModuleName]/Api`
4. Database and external integrations in `Modules/[ModuleName]/Infrastructure`

## Example Flow: Endpoint to Database

Here's how a request flows through the architecture using SignUp as an example:

### 1. API Endpoint (`SignUpEndpoint.cs`)
```csharp
// Receives HTTP POST request at /sign-up
endpointRouteBuilder.MapPost("sign-up", 
    async (SignUpCommand request, ISender sender) => 
    {
        var result = await sender.Send(request);
        return Results.Ok(result);
    })
```

### 2. Command/Query (`SignUpCommand.cs`)
```csharp
// Command object with validation and business logic
public record SignUpCommand(string Username, string Email, string InputPassword) : ICommand<Guid>
```

### 3. Command Handler
```csharp
public sealed class Handler : ICommandHandler<SignUpCommand, Guid>
{
    // 1. Validates business rules (email uniqueness)
    // 2. Creates domain entity using factory methods
    // 3. Persists through repository
    // 4. Commits unit of work
    // 5. Publishes integration events
    // 6. Returns result
}
```

### 4. Domain Entity (`IdentityUser.cs`)
```csharp
// Contains business logic and invariants
IdentityUser.Create(username, email, Password.Create(inputPassword))
```

### 5. Repository (`IdentityUserRepository.cs`)
```csharp
// Abstracts data access, inherits from base Repository<T>
await _identityUserRepository.AddAsync(identityUser, cancellationToken);
```

### 6. DbContext & Database
```csharp
// Entity Framework Core handles SQL generation and execution
// PostgreSQL stores the data
await _unitOfWork.CommitAsync(cancellationToken);
```

### 7. Integration Events
```csharp
// Publishes events for other modules to consume
await _publisher.Publish(new IdentityUserCreatedEvent(...));
```

## Testing
Currently no test projects are configured. When adding tests, create them in the `tests` directory following the module structure.