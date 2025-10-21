# MealMind

A modular nutrition tracking and AI chat platform built with .NET 9, Clean Architecture, and RAG (Retrieval-Augmented Generation).

## 📋 Overview

MealMind is a modular monolith that combines nutrition tracking with an AI-powered chat assistant using RAG. The system helps users track their nutrition goals and provides intelligent, context-aware nutritional advice through semantic search and local LLM integration.

## 🏗️ Architecture

The application follows **Clean Architecture** principles with **Domain-Driven Design (DDD)** and is organized into three main modules:

### Modules

- **Identity Module**: User authentication and management
- **Nutrition Module**: User profiles, personal data, and nutrition tracking
- **AiChat Module**: Conversational AI with RAG-powered semantic search

Each module follows a layered architecture:
```
Module/
├── Api/              # HTTP endpoints
├── Application/      # Use cases (CQRS commands & queries)
├── Domain/           # Business entities, value objects, domain events
└── Infrastructure/   # Database access, external services
```

## 🚀 Tech Stack

### Backend Framework
- **.NET 9** with ASP.NET Core
- **C#** with nullable reference types
- **Entity Framework Core** for data access

### Database
- **PostgreSQL** with **pgvector** extension
- Multi-module database contexts
- EF Core migrations

### AI & RAG
- **Ollama** (llama3.2:3b) for local LLM inference
- **Semantic Kernel** for AI integration
- **pgvector** for storing 768-dimensional embeddings
- **nomic-embed-text** model for embeddings
- Document chunking and semantic search

### Design Patterns & Architecture
- **Clean Architecture** (Domain, Application, Infrastructure, API layers)
- **Domain-Driven Design** (Aggregates, Value Objects, Domain Events)
- **CQRS** (Command Query Responsibility Segregation) via MediatR
- **Repository Pattern**
- **Modular Monolith**

### DevOps
- **Docker** & **Docker Compose** for containerization
- PostgreSQL container with pgvector
- Ollama container for LLM hosting

## ✨ Features

### Nutrition Module
- User profile management
- Personal data tracking (weight, height, activity level)
- Nutrition targets (calories, macros, water intake)
- Customizable nutritional goals

### AiChat Module (RAG Implementation)
- **Document Processing:**
  - Text chunking (500-800 token chunks with overlap)
  - 768-dimensional embedding generation using Ollama
  - Storage in PostgreSQL with pgvector

- **Semantic Search:**
  - Cosine similarity search for relevant context
  - Document retrieval based on user questions

- **AI Chat:**
  - Context-aware responses using retrieved documents
  - Local LLM (llama3.2:3b) via Ollama
  - No external API costs or data privacy concerns

- **Document Types:**
  - **RagDocument**: Global knowledge base (nutrition facts, cooking tips)
  - **ConversationDocument**: User-specific documents (diet summaries, workout logs)

## 🛠️ Getting Started

### Prerequisites

- **.NET 9 SDK** ([Download](https://dotnet.microsoft.com/download/dotnet/9.0))
- **Docker** and **Docker Compose** ([Download](https://www.docker.com/products/docker-desktop))
- **Git**

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/MealMind.git
   cd MealMind
   ```

2. **Start Docker containers (PostgreSQL + Ollama)**
   ```bash
   docker-compose up -d
   ```

3. **Pull required Ollama models**
   ```bash
   # Pull chat model
   docker exec -it MealMind.Ollama ollama pull llama3.2:3b

   # Pull embedding model
   docker exec -it MealMind.Ollama ollama pull nomic-embed-text
   ```

4. **Apply database migrations**
   ```bash
   # Identity module
   dotnet ef database update --project src/Modules/Identity/MealMind.Modules.Identity.Infrastructure --startup-project src/Bootstrapper/MealMind.Bootstrapper

   # Nutrition module
   dotnet ef database update --project src/Modules/Nutrition/MealMind.Modules.Nutrition.Infrastructure --startup-project src/Bootstrapper/MealMind.Bootstrapper

   # AiChat module
   dotnet ef database update --project src/Modules/AiChat/MealMind.Modules.AiChat.Infrastructure --startup-project src/Bootstrapper/MealMind.Bootstrapper
   ```

5. **Run the application**
   ```bash
   dotnet run --project src/Bootstrapper/MealMind.Bootstrapper
   ```

6. **Access the API**
   - Swagger UI: `https://localhost:5001/swagger` (or configured port)

## 🧪 Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=MealMind;Username=postgres;Password=postgres"
  },
  "llm": {
    "ChatModel": "llama3.2:3b",
    "EmbedModel": "nomic-embed-text",
    "Uri": "http://localhost:11434"
  }
}
```

### Environment-Specific Configuration

- **Local Development**: Uses `appsettings.json` with `localhost`
- **Docker Deployment**: Uses `appsettings.Production.json` with container service names

## 📚 RAG Implementation Details

### How RAG Works in MealMind

1. **Document Ingestion:**
   - Documents are split into chunks (~500 tokens)
   - Each chunk is embedded using `nomic-embed-text` (768 dimensions)
   - Chunks stored in PostgreSQL with pgvector

2. **Query Processing:**
   - User asks a question (e.g., "How much protein do I need?")
   - Question is embedded using the same model
   - Cosine similarity search finds top-k relevant chunks

3. **Response Generation:**
   - Top relevant chunks are included as context
   - LLM receives: system prompt + retrieved context + user question
   - LLM generates accurate response based on provided context

### Chunking Strategy

- **Fixed-size chunks** with overlap for context preservation
- **Chunk size**: 500-800 tokens
- **Overlap**: 100-200 tokens
- Uses `Microsoft.SemanticKernel.Text.TextChunker`

See `/LearningMaterials/RAG-Chunking-Strategies.md` for detailed analysis.

## 🗂️ Project Structure

```
MealMind/
├── src/
│   ├── Bootstrapper/                    # Application entry point
│   ├── Modules/
│   │   ├── Identity/                    # User authentication
│   │   ├── Nutrition/                   # Nutrition tracking
│   │   └── AiChat/                      # AI chat with RAG
│   └── Shared/                          # Cross-cutting concerns
├── LearningMaterials/                   # Architecture decisions & guides
├── docker-compose.yml                   # Docker services config
└── MealMind.sln                         # Solution file
```

## 🧩 Key Design Decisions

### Why Modular Monolith?
- Logical separation of business domains
- Easier deployment than microservices initially
- Clear boundaries for future extraction to microservices
- Shared infrastructure reduces duplication

### Why Local LLM (Ollama)?
- **Privacy**: All data stays local
- **Cost**: No API fees
- **Learning**: Understand how LLMs work
- **Control**: Full control over model and configuration

### Why pgvector?
- Native PostgreSQL extension for vector storage
- No separate vector database needed
- ACID compliance for data consistency
- Excellent integration with EF Core

### Why RAG?
- LLMs have knowledge cutoffs and can hallucinate
- RAG provides up-to-date, specific nutritional information
- Combines benefits of retrieval systems and generative AI
- Better accuracy for domain-specific questions

## 📖 Learning Resources

The `/LearningMaterials` directory contains detailed documentation:

- `RAG-Chunking-Strategies.md` - Overview of chunking approaches
- `RAG-Document-Chunking-Approaches.md` - Storage pattern comparisons
- `TUnit-Testing-Guide.md` - Testing guide (for future test implementation)
- `Testing-Types-Guide.md` - Overview of testing types
- `Job-Application-Guide.md` - CV and interview preparation

## 🔧 Development

### Adding a New Migration

```bash
# Example for Nutrition module
dotnet ef migrations add MigrationName \
  --project src/Modules/Nutrition/MealMind.Modules.Nutrition.Infrastructure \
  --startup-project src/Bootstrapper/MealMind.Bootstrapper
```

### Building the Solution

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

## 🐳 Docker Services

### PostgreSQL
- **Image**: `postgres:latest` with pgvector
- **Port**: 5432
- **Database**: MealMind

### Ollama
- **Port**: 11434
- **Models**: llama3.2:3b, nomic-embed-text
- **GPU Support**: Configured for NVIDIA GPUs (optional)

### Managing Containers

```bash
# Start services
docker-compose up -d

# Stop services
docker-compose down

# View logs
docker-compose logs -f

# Check Ollama models
docker exec -it MealMind.Ollama ollama list
```

## 🚧 Future Improvements

- [ ] Comprehensive test suite (unit, integration, E2E)
- [ ] Frontend UI (Blazor or React)
- [ ] Azure/AWS deployment
- [ ] OpenFoodFacts API integration
- [ ] Meal planning features
- [ ] User authentication UI
- [ ] Mobile app integration
- [ ] Advanced RAG techniques (re-ranking, hybrid search)
- [ ] CI/CD pipeline

## 🤝 Contributing

This is a personal learning project, but suggestions and feedback are welcome! Feel free to:
- Open an issue for bugs or feature suggestions
- Submit a pull request with improvements
- Share your thoughts on the architecture

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👤 Author

**Michał Drozd**
- Email: michaldrozd03@gmail.com
- LinkedIn: [linkedin.com/in/your-profile](https://linkedin.com/in/your-profile)
- GitHub: [@yourusername](https://github.com/yourusername)

## 🙏 Acknowledgments

- **DevMentors** - Modular Monolith and SOLID WEB API courses
- **Microsoft** - Semantic Kernel and .NET documentation
- **Ollama** - Local LLM hosting
- **pgvector** - PostgreSQL vector extension

---

Built with ❤️ using .NET 9, Clean Architecture, and RAG
