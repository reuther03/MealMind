# Commands Reference

This document contains frequently used commands for MealMind development.

## Table of Contents
- [Docker Commands](#docker-commands)
- [Database Commands](#database-commands)
- [Entity Framework Migrations](#entity-framework-migrations)
- [Ollama Commands](#ollama-commands)
- [Build & Run](#build--run)

---

## Docker Commands

### Start All Services
```bash
docker-compose up -d
```

### Stop All Services (Keep Data)
```bash
docker-compose down
```

### Stop All Services and Delete Volumes
```bash
docker-compose down -v
```
**⚠️ Warning:** This deletes ALL data including Ollama models!

### View Container Logs
```bash
# View all logs
docker-compose logs

# Follow logs (live)
docker-compose logs -f

# Specific service
docker logs MealMind.Postgres
docker logs MealMind.Ollama
docker logs MealMind.Api
```

### Check Running Containers
```bash
docker ps
```

---

## Database Commands

### Reset PostgreSQL Database Only (Preserve Ollama Models)

**Option 1: Delete Bind Mount Directory**
```bash
# Stop containers
docker-compose down

# Delete PostgreSQL data directory
rm -rf .container/postgres
# Windows PowerShell:
Remove-Item -Recurse -Force .container\postgres
# Windows CMD:
rmdir /s /q .container\postgres

# Start containers
docker-compose up -d

# Apply migrations (see below)
```

**Option 2: Drop Database Using SQL (Without Stopping Containers)**
```bash
# Connect to PostgreSQL
docker exec -it MealMind.Postgres psql -U postgres

# Drop and recreate database
DROP DATABASE "MealMind";
CREATE DATABASE "MealMind";
\q

# Then apply migrations
```

### Connect to PostgreSQL
```bash
# Using psql
docker exec -it MealMind.Postgres psql -U postgres -d MealMind

# Common psql commands:
\l          # List databases
\c MealMind # Connect to database
\dt         # List tables
\q          # Quit
```

### Check Database Schema
```sql
-- List all tables in nutrition schema
SELECT table_name
FROM information_schema.tables
WHERE table_schema = 'nutrition';

-- Check column precision for troubleshooting
SELECT
    column_name,
    data_type,
    numeric_precision,
    numeric_scale
FROM information_schema.columns
WHERE table_schema = 'nutrition'
  AND table_name = 'FoodEntry'
  AND column_name LIKE 'Total%'
ORDER BY column_name;
```

---

## Entity Framework Migrations

### Apply All Pending Migrations

**Nutrition Module:**
```bash
dotnet ef database update \
  --project src/Modules/Nutrition/MealMind.Modules.Nutrition.Infrastructure \
  --startup-project src/Bootstrapper/MealMind.Bootstrapper \
  --context NutritionDbContext
```

**Identity Module:**
```bash
dotnet ef database update \
  --project src/Modules/Identity/MealMind.Modules.Identity.Infrastructure \
  --startup-project src/Bootstrapper/MealMind.Bootstrapper \
  --context IdentityDbContext
```

**AiChat Module:**
```bash
dotnet ef database update \
  --project src/Modules/AiChat/MealMind.Modules.AiCha![img.png](img.png)t.Infrastructure \
  --startup-project src/Bootstrapper/MealMind.Bootstrapper \
  --context AiChatDbContext
```

### Create New Migration

**Nutrition Module:**
```bash
dotnet ef migrations add MigrationName \
  --project src/Modules/Nutrition/MealMind.Modules.Nutrition.Infrastructure \
  --startup-project src/Bootstrapper/MealMind.Bootstrapper \
  --context NutritionDbContext
```

**Identity Module:**
```bash
dotnet ef migrations add MigrationName \
  --project src/Modules/Identity/MealMind.Modules.Identity.Infrastructure \
  --startup-project src/Bootstrapper/MealMind.Bootstrapper \
  --context IdentityDbContext
```

**AiChat Module:**
```bash
dotnet ef migrations add MigrationName \
  --project src/Modules/AiChat/MealMind.Modules.AiChat.Infrastructure \
  --startup-project src/Bootstrapper/MealMind.Bootstrapper \
  --context AiChatDbContext
```

### Remove Last Migration (Not Applied Yet)
```bash
dotnet ef migrations remove \
  --project src/Modules/Nutrition/MealMind.Modules.Nutrition.Infrastructure \
  --startup-project src/Bootstrapper/MealMind.Bootstrapper \
  --context NutritionDbContext
```

### List All Migrations
```bash
dotnet ef migrations list \
  --project src/Modules/Nutrition/MealMind.Modules.Nutrition.Infrastructure \
  --startup-project src/Bootstrapper/MealMind.Bootstrapper \
  --context NutritionDbContext
```

---

## Ollama Commands

### Pull Models After Fresh Install
```bash
# Pull chat model (llama3.2:3b)
docker exec -it MealMind.Ollama ollama pull llama3.2:3b

# Pull embedding model (nomic-embed-text)
docker exec -it MealMind.Ollama ollama pull nomic-embed-text
```

### List Installed Models
```bash
docker exec -it MealMind.Ollama ollama list
```

### Test Model
```bash
# Interactive chat
docker exec -it MealMind.Ollama ollama run llama3.2:3b

# Type your message and press Enter
# Type /bye to exit
```

### Remove Model
```bash
docker exec -it MealMind.Ollama ollama rm llama3.2:3b
```

### Check Ollama Logs
```bash
docker logs MealMind.Ollama -f
```

---

## Build & Run

### Build Solution
```bash
dotnet build
```

### Run Application (Local Development)
```bash
dotnet run --project src/Bootstrapper/MealMind.Bootstrapper
```

### Run Application (Docker)
```bash
docker-compose up --build
```

### Clean Solution
```bash
dotnet clean
```

### Restore NuGet Packages
```bash
dotnet restore
```

---

## Common Workflows

### Fresh Start (Reset Everything)
```bash
# 1. Stop and remove all containers and volumes
docker-compose down -v

# 2. Start containers
docker-compose up -d

# 3. Pull Ollama models
docker exec -it MealMind.Ollama ollama pull llama3.2:3b
docker exec -it MealMind.Ollama ollama pull nomic-embed-text

# 4. Apply migrations
dotnet ef database update --project src/Modules/Nutrition/MealMind.Modules.Nutrition.Infrastructure --startup-project src/Bootstrapper/MealMind.Bootstrapper --context NutritionDbContext
dotnet ef database update --project src/Modules/Identity/MealMind.Modules.Identity.Infrastructure --startup-project src/Bootstrapper/MealMind.Bootstrapper --context IdentityDbContext
dotnet ef database update --project src/Modules/AiChat/MealMind.Modules.AiChat.Infrastructure --startup-project src/Bootstrapper/MealMind.Bootstrapper --context AiChatDbContext

# 5. Run application
dotnet run --project src/Bootstrapper/MealMind.Bootstrapper
```

### Reset Database Only (Keep Ollama Models)
```bash
# 1. Stop containers
docker-compose down

# 2. Delete PostgreSQL data
rm -rf .container/postgres

# 3. Start containers
docker-compose up -d

# 4. Apply migrations
dotnet ef database update --project src/Modules/Nutrition/MealMind.Modules.Nutrition.Infrastructure --startup-project src/Bootstrapper/MealMind.Bootstrapper --context NutritionDbContext
dotnet ef database update --project src/Modules/Identity/MealMind.Modules.Identity.Infrastructure --startup-project src/Bootstrapper/MealMind.Bootstrapper --context IdentityDbContext
dotnet ef database update --project src/Modules/AiChat/MealMind.Modules.AiChat.Infrastructure --startup-project src/Bootstrapper/MealMind.Bootstrapper --context AiChatDbContext
```

### Add New Migration and Apply
```bash
# 1. Create migration
dotnet ef migrations add MyMigrationName \
  --project src/Modules/Nutrition/MealMind.Modules.Nutrition.Infrastructure \
  --startup-project src/Bootstrapper/MealMind.Bootstrapper \
  --context NutritionDbContext

# 2. Review the generated migration file in:
# src/Modules/Nutrition/MealMind.Modules.Nutrition.Infrastructure/Database/Migrations/

# 3. Apply migration
dotnet ef database update \
  --project src/Modules/Nutrition/MealMind.Modules.Nutrition.Infrastructure \
  --startup-project src/Bootstrapper/MealMind.Bootstrapper \
  --context NutritionDbContext
```

---

## Troubleshooting Commands

### Check Database Connection
```bash
docker exec -it MealMind.Postgres pg_isready -U postgres
```

### View Database Size
```sql
SELECT pg_size_pretty(pg_database_size('MealMind'));
```

### Check Ollama API
```bash
curl http://localhost:11434/api/tags
```

### Check Application Health
```bash
curl http://localhost:5000/health
```

### Clear Docker Cache (If Build Issues)
```bash
docker system prune -a
```

---

## Notes

- Always use `--context` parameter when working with EF migrations since we have multiple DbContexts
- The `-v` flag in `docker-compose down -v` removes ALL volumes including Ollama models
- PostgreSQL data is stored in `.container/postgres` (bind mount, not Docker volume)
- Ollama models are stored in `ollama_data` Docker volume
- Default connection string: `Host=localhost;Port=5432;Database=MealMind;Username=postgres;Password=`