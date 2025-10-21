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

## 🚧 Future Improvements

- [ ] Comprehensive test suite (unit, integration, E2E)
- [ ] Frontend UI
- [ ] Azure/AWS deployment
- [ ] Meal planning features
- [ ] Advanced RAG techniques (re-ranking, hybrid search)
- [ ] CI/CD pipeline

## 👤 Author
- LinkedIn: [linkedin.com/in/your-profile](https://www.linkedin.com/in/michal-drozd-38750a32b/)