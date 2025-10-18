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
  - AiChat: Conversational AI with RAG (Retrieval-Augmented Generation)
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
- PostgreSQL with pgvector extension on port 5432 (configured in docker-compose.yml)
- Connection string in appsettings.json
- Each module manages its own DbContext and migrations
- pgvector extension used for AI embeddings and semantic search in AiChat module

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

## AI Chat Module (RAG Implementation)

### Architecture
The AiChat module implements RAG (Retrieval-Augmented Generation) using:
- **Ollama** for local LLM inference (llama3.2:3b) and embeddings (nomic-embed-text)
- **pgvector** for storing and querying 768-dimensional embeddings
- **Microsoft.SemanticKernel.Connectors.Ollama** for AI integration
- **Table Per Concrete Type (TPC)** inheritance mapping for document entities

### Document Storage
Two types of documents for RAG context:
- **RagDocument**: Global knowledge base (nutrition facts, cooking tips) - searchable across all conversations
- **ConversationDocument**: User-specific documents (diet summaries, workout logs) - attached to specific conversations

Both use TPC inheritance from abstract `Document` base class:
```csharp
public abstract class Document : Entity<Guid>
{
    public Guid DocumentGroupId { get; }  // Logical grouping for chunks from same document
    public int ChunkIndex { get; }        // Position in original document (0, 1, 2...)
    public string Title { get; }
    public string Content { get; }         // Individual chunk text
    public Vector Embedding { get; }       // 768-dim vector for semantic search
    public DateTime AttachedAt { get; }
}
```

### Chunking & Embedding Flow
1. **Chunking**: `IChunkingService` splits documents into ~500 token chunks using `Microsoft.SemanticKernel.Text.TextChunker`
2. **Embedding**: `IEmbeddingService` generates 768-dim vectors using Ollama's `nomic-embed-text` model
3. **Storage**: Each chunk stored as separate row with embedding in pgvector column
4. **Semantic Search**: cosine similarity search finds relevant chunks for user questions

### Cross-Module Communication
Other modules (Nutrition, Training) can attach documents to conversations:
```csharp
// Nutrition Module generates diet summary
var dietDoc = GenerateDietSummary(userId);

// Send to AiChat Module
await _sender.Send(new AttachDocumentCommand(
    conversationId,
    "Diet Summary",
    dietDoc,
    DocumentSource.NutritionData
));

// AiChat chunks, embeds, and stores
// User's chat AI now has access to diet data via semantic search
```

### Learning Resources
See `docs/learning/` for detailed explanations:
- `RAG-Chunking-Strategies.md` - Overview of chunking strategies
- `RAG-Document-Chunking-Approaches.md` - Comparison of storage approaches (logical grouping vs full document + chunks)

## Ollama Configuration

### Running Locally (Development)
```bash
# API runs on host (dotnet run), connects to containers via localhost
# appsettings.json:
"llm": {
  "ChatModel": "llama3.2:3b",
  "EmbedModel": "nomic-embed-text",
  "Uri": "http://localhost:11434"
}
```

### Running in Docker (Production)
```bash
# API runs in container, connects via service names
# docker-compose.yml environment variables:
llm__Uri: "http://mealmind.ollama:11434"
```

### Managing Ollama Container
```bash
# Pull model into container
docker exec -it MealMind.Ollama ollama pull llama3.2:3b

# List available models
docker exec -it MealMind.Ollama ollama list

# Keep model loaded (optional, OLLAMA_KEEP_ALIVE handles this)
docker exec -it MealMind.Ollama ollama run llama3.2:3b
```

## Testing
Currently no test projects are configured. When adding tests, create them in the `tests` directory following the module structure.