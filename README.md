# MealMind

A modular nutrition tracking and AI chat platform built with .NET 9, Clean Architecture, and RAG (Retrieval-Augmented Generation).

## 📋 Overview

MealMind is a modular monolith that combines nutrition tracking with an AI-powered chat assistant using RAG. The system helps users track their nutrition goals and provides intelligent, context-aware nutritional advice through semantic search and local LLM integration.

**Key Highlights:**
- 🤖 Local AI with no external API costs
- 🔒 Privacy-focused (all data stays local)
- 🚀 GPU-accelerated Ollama inference
- 📚 RAG with semantic search using pgvector
- 🏗️ Clean Architecture with DDD
- 🐳 Fully containerized with Docker

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
- **Ollama** with **Qwen 3** models for local LLM inference
  - Chat: `qwen3:4b` (reliable JSON generation, strong instruction following)
  - Embeddings: `qwen3-embedding:0.6b` (1024 dimensions)
- **Semantic Kernel** for AI integration
- **pgvector** for storing 1024-dimensional embeddings
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

#### Document Processing Pipeline
1. **Text Chunking**: Documents split into ~500 token chunks using Semantic Kernel's TextChunker
2. **Embedding Generation**: Each chunk converted to 1024-dimensional vectors using `qwen3-embedding:0.6b` model
3. **Vector Storage**: Embeddings stored in PostgreSQL with pgvector extension
4. **Document Grouping**: Chunks linked via `DocumentGroupId` for logical document reconstruction

#### Semantic Search Flow
1. User submits question → Generate query embedding
2. **Cosine similarity search** against pgvector database (`<=>` operator)
3. Retrieve top 3-6 most relevant document chunks
4. Inject retrieved context into LLM system prompt
5. LLM generates structured JSON response with facts from documents

#### Document Types (TPC Inheritance)
- **RagDocument**: Global knowledge base (nutrition facts, cooking tips, meal planning)
  - Searchable across all conversations
  - Seeded from markdown files in `seed-data/`
- **ConversationDocument**: User-specific context (diet summaries, workout logs)
  - Scoped to individual conversations
  - Attached dynamically by other modules

#### Structured Response Generation
- **ResponseManager** service formats LLM output as structured JSON
- Schema: `{ Title, Paragraphs, KeyPoints, Sources }`
- Detailed paragraphs (100-250 words) with specific facts and numbers
- Short key points (10-30 words) summarizing main takeaways
- Exact source document citations to prevent hallucination

## 🚀 Getting Started

### Prerequisites
- **.NET 9 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Docker Desktop** - [Download](https://www.docker.com/products/docker-desktop)
- **NVIDIA GPU** (optional but recommended for faster LLM inference)
- **NVIDIA Container Toolkit** (if using GPU)

### Quick Start

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/MealMind.git
   cd MealMind
   ```

2. **Start Docker containers**
   ```bash
   docker-compose up -d
   ```
   This starts:
   - PostgreSQL with pgvector on port 5432
   - Ollama with GPU support on port 11434

3. **Pull Ollama models**
   ```bash
   docker exec MealMind.Ollama ollama pull qwen3:4b
   docker exec MealMind.Ollama ollama pull qwen3-embedding:0.6b
   ```

4. **Verify GPU usage** (optional)
   ```bash
   docker exec MealMind.Ollama ollama ps
   # Should show "100% GPU" for processor
   ```

5. **Apply database migrations**
   ```bash
   dotnet ef database update --project src/Modules/AiChat/MealMind.Modules.AiChat.Infrastructure --startup-project src/Bootstrapper/MealMind.Bootstrapper
   dotnet ef database update --project src/Modules/Nutrition/MealMind.Modules.Nutrition.Infrastructure --startup-project src/Bootstrapper/MealMind.Bootstrapper
   ```

6. **Run the application**
   ```bash
   dotnet run --project src/Bootstrapper/MealMind.Bootstrapper
   ```
   API available at: `http://localhost:5000`

### Configuration

Edit `src/Bootstrapper/MealMind.Bootstrapper/appsettings.json`:

```json
{
  "postgres": {
    "connectionString": "Host=localhost;Port=5432;Database=MealMind;Username=postgres;Password="
  },
  "llm": {
    "ChatModel": "qwen3:4b",
    "EmbedModel": "qwen3-embedding:0.6b",
    "Uri": "http://localhost:11434"
  }
}
```

**For Docker deployment**, change `llm.Uri` to `http://mealmind.ollama:11434`

## 📚 RAG System Details

### Seeding Documents

1. Place markdown files in `seed-data/` directory
2. Run the application - documents are automatically:
   - Chunked into ~500 token segments
   - Embedded using qwen3-embedding:0.6b (1024 dimensions)
   - Stored in PostgreSQL with pgvector

**Current seeded topics:**
- Basic Nutrition Guidelines
- Calorie Calculation & TDEE
- Macro Tracking & Adjustment
- Muscle Gain Nutrition
- Fat Loss Nutrition
- Supplements Guide
- Meal Planning & Timing
- Micronutrients Overview
- Sleep, Stress & Nutrition
- And more...

### Vector Search Configuration

**Similarity Metric**: Cosine distance (`<=>` operator in pgvector)
- Range: 0 (identical) to 2 (opposite)
- Lower values = more relevant documents

**Retrieval Settings**:
- Top K: 3-6 chunks (configurable in `DocumentRepository`)
- No hard similarity threshold (retrieves best matches)
- Can be tuned based on use case

### System Prompt Strategy

The RAG system uses a carefully crafted system prompt that:
1. **Shows concrete examples** of desired output format
2. **Explicitly states what NOT to do** (negative examples)
3. **Provides exact source document titles** to prevent hallucination
4. **Balances detail requirements**:
   - Paragraphs: 100-250 words with specific facts
   - Key Points: 10-30 words, brief summaries
5. **Enforces strict JSON format** with validation

### LLM Performance

**Recommended Model**: `qwen3:4b` (GPU-accelerated) ✅
- **Speed**: ~2-5 seconds per response with GPU
- **Quality**: Excellent for factual Q&A with RAG context
- **Strengths**:
  - **Reliable JSON generation**: Consistent structured output
  - **Strong instruction following**: Rarely hallucinates sources
  - **No format errors**: Proper JSON schema adherence
  - **Better than llama3.2:3b**: Fewer retry attempts needed

**Why Qwen 3 over Llama 3.2:3b?**
- Llama 3.2:3b had frequent issues:
  - Hallucinated document sources not in the knowledge base
  - Inconsistent JSON structure (malformed syntax)
  - Returned dummy strings instead of proper responses
  - Required complex retry logic in ResponseManager
- Qwen 3:4b solved all these issues while maintaining similar speed

**Alternative Models** (if you need different trade-offs):
- `qwen3:8b` - Even better quality, slightly slower
- `mistral:7b` - Good structured output, different style
- `llama3.1:8b` - Strong reasoning but less reliable formatting

## 🏗️ Architecture Decisions

### Why Modular Monolith?
- Simpler deployment than microservices
- Module independence for future extraction
- Shared infrastructure (DB, auth) without network overhead

### Why TPC Inheritance for Documents?
- **Table Per Concrete Type** allows different document types (RagDocument, ConversationDocument)
- Separate tables for performance
- Shared base class for polymorphic queries
- Better than TPH (Table Per Hierarchy) for pgvector indexing

### Why Local LLM (Ollama)?
- **Privacy**: All data stays on-premises
- **Cost**: No API fees (OpenAI, Anthropic)
- **Control**: Full customization of models and parameters
- **Offline**: Works without internet

### Why pgvector?
- **Performance**: Native PostgreSQL extension, no separate vector DB
- **Simplicity**: One database for relational + vector data
- **Proven**: Production-ready, used by major companies
- **Features**: HNSW indexing for fast approximate search

## 🧪 Example API Usage

### Create a Conversation
```bash
POST http://localhost:5000/conversations/create
Content-Type: application/json

{
  "userId": "user-guid-here"
}
```

### Ask a Question (RAG-powered)
```bash
POST http://localhost:5000/get-chat-response
Content-Type: application/json

{
  "conversationId": "conversation-guid",
  "prompt": "How much protein should I eat for muscle gain?"
}
```

**Response**:
```json
{
  "value": {
    "title": "Protein Requirements for Muscle Gain",
    "paragraphs": [
      "For strength and hypertrophy training, protein intake should be 1.6-2.2 grams per kilogram of body weight according to the Basic Nutrition Guidelines. This range maximizes muscle protein synthesis and supports recovery after resistance training.",
      "The protein should be spread across 3-5 meals per day for optimal absorption. Complete protein sources like eggs, meat, fish, dairy, and soy are preferred as they contain all essential amino acids."
    ],
    "keyPoints": [
      "1.6-2.2 g/kg body weight daily for muscle gain",
      "Spread across 3-5 meals for best absorption",
      "Use complete protein sources",
      "Higher than sedentary needs (0.8-1.0 g/kg)"
    ],
    "sources": ["Basic Nutrition Guidelines"]
  }
}
```

## 🚧 Future Improvements

- [ ] Comprehensive test suite (unit, integration, E2E)
- [ ] Frontend UI (React/Next.js)
- [ ] Azure/AWS deployment guide
- [ ] Meal planning features with AI suggestions
- [ ] Advanced RAG: Re-ranking, hybrid search, query expansion
- [ ] CI/CD pipeline (GitHub Actions)
- [ ] Docker production image optimization
- [ ] Monitoring & logging (Seq, Application Insights)
- [ ] Rate limiting & authentication middleware
- [ ] Document versioning and update tracking

## 📖 Learning Resources

See `docs/learning/` for detailed explanations:
- `RAG-Chunking-Strategies.md` - Overview of chunking strategies
- `RAG-Document-Chunking-Approaches.md` - Storage architecture comparison

## 🤝 Contributing

This is a personal learning project, but suggestions and discussions are welcome via issues.

## 📄 License

This project is for educational purposes.

## 👤 Author

**Michał Drozd**
- LinkedIn: [linkedin.com/in/michal-drozd-38750a32b](https://www.linkedin.com/in/michal-drozd-38750a32b/)
- Focus: Clean Architecture, DDD, RAG, .NET Development