# RAG Document Chunking Approaches

This document explains two different approaches for storing chunked documents in a RAG (Retrieval-Augmented Generation) system.

## Background

When implementing RAG with vector embeddings, large documents need to be split into smaller chunks because:
1. LLMs have token limits (e.g., 4096 tokens)
2. Smaller chunks = more precise semantic search
3. Only send relevant chunks to the LLM, not entire documents

The question is: **How do we track which chunks belong to the same original document?**

---

## Approach 1: Logical Grouping (Simpler) ✅ RECOMMENDED

### Concept
Don't store the full original document. Instead, use a **logical grouping GUID** to link chunks together.

### Implementation

```csharp
public abstract class Document : Entity<Guid>
{
    public Guid ParentDocumentId { get; protected set; }  // Logical grouping ID
    public int ChunkIndex { get; protected set; }         // Position in original doc (0, 1, 2...)
    public string Title { get; protected set; }           // Same title for all chunks
    public string Content { get; protected set; }         // Individual chunk content
    public Vector? Embedding { get; protected set; }      // Embedding for this chunk
    public DateTime CreatedAt { get; protected set; }
}

public class RagDocument : Document
{
    public RagDocumentCategory Category { get; private set; }

    public static RagDocument Create(
        Guid parentDocumentId,      // Same ID for all chunks from same document
        int chunkIndex,
        string title,
        string content,
        Vector? embedding,
        RagDocumentCategory category)
    {
        return new(Guid.NewGuid(), parentDocumentId, chunkIndex, title, content, embedding, category);
    }
}
```

### Usage Example

```csharp
// When seeding/chunking a document:
var fullDocument = "Long nutrition guide about protein, carbs, and fats...";
var parentId = Guid.NewGuid();  // Generate logical grouping ID

var chunks = chunkingService.Chunk(fullDocument);  // ["Protein info...", "Carbs info...", "Fats info..."]

for (int i = 0; i < chunks.Count; i++)
{
    var embedding = await embeddingService.GenerateEmbeddingAsync(chunks[i]);

    var doc = RagDocument.Create(
        parentDocumentId: parentId,                    // Same for all chunks
        chunkIndex: i,                                  // 0, 1, 2, 3...
        title: "Complete Nutrition Guide",              // Same title
        content: chunks[i],                             // Different content per chunk
        embedding: embedding,
        category: RagDocumentCategory.NutritionFacts
    );

    await repository.AddAsync(doc);
}
```

### Database Result

```
RagDocument table:
┌─────────────┬──────────────────────┬────────────┬──────────────────────────┬─────────────────┬─────────────┐
│ Id          │ ParentDocumentId     │ ChunkIndex │ Title                    │ Content         │ Embedding   │
├─────────────┼──────────────────────┼────────────┼──────────────────────────┼─────────────────┼─────────────┤
│ guid-chunk-1│ parent-guid-1        │ 0          │ Complete Nutrition Guide │ Protein info... │ [0.23, ...] │
│ guid-chunk-2│ parent-guid-1        │ 1          │ Complete Nutrition Guide │ Carbs info...   │ [0.41, ...] │
│ guid-chunk-3│ parent-guid-1        │ 2          │ Complete Nutrition Guide │ Fats info...    │ [0.18, ...] │
│ guid-chunk-4│ parent-guid-2        │ 0          │ Cooking Tips             │ Chicken prep... │ [0.52, ...] │
└─────────────┴──────────────────────┴────────────┴──────────────────────────┴─────────────────┴─────────────┘
```

### Querying

```csharp
// Get all chunks from a specific document
var allChunks = await context.RagDocuments
    .Where(x => x.ParentDocumentId == parentId)
    .OrderBy(x => x.ChunkIndex)
    .ToListAsync();

// Reconstruct original document (if needed)
var reconstructed = string.Join("\n", allChunks.Select(x => x.Content));

// Semantic search (normal RAG usage)
var relevantChunks = await context.RagDocuments
    .OrderBy(x => x.Embedding.CosineDistance(questionEmbedding))
    .Take(3)
    .ToListAsync();
```

### Pros
- ✅ Simple database schema
- ✅ No storage duplication
- ✅ Fast semantic search (search on chunks directly)
- ✅ Don't need full original document after chunking

### Cons
- ❌ Can't view/display the original full document
- ❌ Can't re-chunk with different strategy later
- ❌ ParentDocumentId doesn't reference a real entity (just a logical ID)

### When to Use
- ✅ You only care about semantic search, not displaying originals
- ✅ Documents are generated/synthesized (not user uploads)
- ✅ You want simplicity and performance
- ✅ **This is the RECOMMENDED approach for most use cases**

---

## Approach 2: Full Document + Chunks (More Complex)

### Concept
Store both the **full original document** AND its **chunks** in separate tables with a proper foreign key relationship.

### Implementation

```csharp
// Aggregate root - stores the full original document
public class RagDocumentMetadata : AggregateRoot<Guid>
{
    public string Title { get; private set; }
    public string FullContent { get; private set; }  // Complete unchunked text
    public RagDocumentCategory Category { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly IList<RagDocumentChunk> _chunks = [];
    public IReadOnlyList<RagDocumentChunk> Chunks => _chunks.AsReadOnly();

    public static RagDocumentMetadata Create(string title, string fullContent, RagDocumentCategory category)
    {
        return new RagDocumentMetadata
        {
            Id = Guid.NewGuid(),
            Title = title,
            FullContent = fullContent,
            Category = category,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void AddChunk(string content, Vector embedding, int chunkIndex)
    {
        var chunk = new RagDocumentChunk
        {
            Id = Guid.NewGuid(),
            ParentDocumentId = this.Id,
            ChunkIndex = chunkIndex,
            Content = content,
            Embedding = embedding
        };
        _chunks.Add(chunk);
    }
}

// Child entity - individual chunks
public class RagDocumentChunk : Entity<Guid>
{
    public Guid ParentDocumentId { get; private set; }  // FK to RagDocumentMetadata
    public int ChunkIndex { get; private set; }
    public string Content { get; private set; }
    public Vector Embedding { get; private set; }
}
```

### Usage Example

```csharp
// When seeding/chunking a document:
var fullDocument = "Long nutrition guide about protein, carbs, and fats...";

// 1. Create metadata entity with full document
var metadata = RagDocumentMetadata.Create(
    title: "Complete Nutrition Guide",
    fullContent: fullDocument,  // Store original
    category: RagDocumentCategory.NutritionFacts
);

// 2. Chunk and add to metadata
var chunks = chunkingService.Chunk(fullDocument);

for (int i = 0; i < chunks.Count; i++)
{
    var embedding = await embeddingService.GenerateEmbeddingAsync(chunks[i]);
    metadata.AddChunk(chunks[i], embedding, i);
}

// 3. Save aggregate (saves metadata + all chunks)
await repository.AddAsync(metadata);
await unitOfWork.CommitAsync();
```

### Database Result

```
RagDocumentMetadata table:
┌──────────────────┬──────────────────────────┬──────────────────────────────────────────┬──────────────┐
│ Id               │ Title                    │ FullContent                              │ Category     │
├──────────────────┼──────────────────────────┼──────────────────────────────────────────┼──────────────┤
│ metadata-guid-1  │ Complete Nutrition Guide │ Long nutrition guide about protein...    │ NutritionFacts│
└──────────────────┴──────────────────────────┴──────────────────────────────────────────┴──────────────┘

RagDocumentChunk table:
┌─────────────┬──────────────────────┬────────────┬─────────────────┬─────────────┐
│ Id          │ ParentDocumentId     │ ChunkIndex │ Content         │ Embedding   │
├─────────────┼──────────────────────┼────────────┼─────────────────┼─────────────┤
│ chunk-guid-1│ metadata-guid-1      │ 0          │ Protein info... │ [0.23, ...] │
│ chunk-guid-2│ metadata-guid-1      │ 1          │ Carbs info...   │ [0.41, ...] │
│ chunk-guid-3│ metadata-guid-1      │ 2          │ Fats info...    │ [0.18, ...] │
└─────────────┴──────────────────────┴────────────┴─────────────────┴─────────────┘
```

### EF Core Configuration

```csharp
public class RagDocumentMetadataConfiguration : IEntityTypeConfiguration<RagDocumentMetadata>
{
    public void Configure(EntityTypeBuilder<RagDocumentMetadata> builder)
    {
        builder.ToTable("RagDocumentMetadata");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FullContent).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();

        // Configure relationship
        builder.HasMany<RagDocumentChunk>()
            .WithOne()
            .HasForeignKey(x => x.ParentDocumentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class RagDocumentChunkConfiguration : IEntityTypeConfiguration<RagDocumentChunk>
{
    public void Configure(EntityTypeBuilder<RagDocumentChunk> builder)
    {
        builder.ToTable("RagDocumentChunks");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.Embedding)
            .HasColumnType("vector(768)")
            .IsRequired();

        // Index for semantic search
        builder.HasIndex(x => x.Embedding)
            .HasMethod("hnsw")
            .HasOperators("vector_cosine_ops");

        // Index for getting all chunks from a document
        builder.HasIndex(x => new { x.ParentDocumentId, x.ChunkIndex });
    }
}
```

### Querying

```csharp
// Get full document with all chunks
var documentWithChunks = await context.RagDocumentMetadata
    .Include(x => x.Chunks)
    .FirstOrDefaultAsync(x => x.Id == metadataId);

// Display original full document
var fullText = documentWithChunks.FullContent;

// Semantic search (search on chunks)
var relevantChunks = await context.RagDocumentChunks
    .OrderBy(x => x.Embedding.CosineDistance(questionEmbedding))
    .Take(3)
    .Include(x => x.ParentDocument)  // Can access metadata
    .ToListAsync();

// Re-chunk all documents with new strategy
var allDocuments = await context.RagDocumentMetadata.ToListAsync();
foreach (var doc in allDocuments)
{
    // Delete old chunks
    context.RagDocumentChunks.RemoveRange(doc.Chunks);

    // Create new chunks with different strategy
    var newChunks = newChunkingService.Chunk(doc.FullContent);
    // ... add new chunks
}
```

### Pros
- ✅ Can display/download original full document
- ✅ Can re-chunk documents with different strategies
- ✅ Proper foreign key relationships (referential integrity)
- ✅ Can track metadata about the original document separately

### Cons
- ❌ More complex database schema
- ❌ Storage duplication (full document + chunks)
- ❌ More entities to manage
- ❌ Slightly slower queries (need joins to get metadata)

### When to Use
- ✅ Users upload documents and want to view originals
- ✅ You need to experiment with different chunking strategies
- ✅ Document metadata is important (author, upload date, file type, etc.)
- ✅ Compliance/audit requirements to keep original documents

---

## Comparison Table

| Feature | Approach 1 (Logical Grouping) | Approach 2 (Full Document + Chunks) |
|---------|-------------------------------|--------------------------------------|
| **Complexity** | Simple | Complex |
| **Storage** | Chunks only | Full document + chunks |
| **Can view original?** | No | Yes |
| **Can re-chunk?** | No | Yes |
| **Foreign key relationship** | No (logical ID) | Yes (proper FK) |
| **Performance** | Faster (no joins) | Slower (needs joins) |
| **Use case** | Generated documents, RAG-only | User uploads, document management |
| **Recommended for MealMind** | ✅ YES | ❌ Not needed |

---

## Recommendation for MealMind

**Use Approach 1 (Logical Grouping)** because:

1. Documents are **generated** from database data (diet summaries, workout logs)
2. Users don't need to view "original documents" - they can view their data in the source module
3. Chunking strategy is fixed (no need to re-chunk)
4. Simpler = easier to maintain
5. Better performance for semantic search

**Only use Approach 2 if:**
- You add a feature for users to upload PDF/text files
- You need to display uploaded documents
- You need to experiment with different chunking strategies

---

## Example: MealMind Diet Document Flow (Approach 1)

```csharp
// 1. Nutrition Module generates diet summary
var dietSummary = """
    ## User Profile
    Weight: 75kg, Height: 180cm
    Target: 2500 kcal, 150g protein

    ## Recent Meals (Last 7 Days)
    - 2025-10-10: Chicken breast (350 kcal, 45g protein)
    - 2025-10-11: Salmon salad (420 kcal, 38g protein)
    ...

    ## Analysis
    Average daily calories: 2300 kcal
    Average daily protein: 140g
    """;

// 2. Send to AiChat Module
await sender.Send(new AttachDocumentCommand(
    conversationId,
    "Diet Summary - Last 7 Days",
    dietSummary,
    DocumentSource.NutritionData
));

// 3. AiChat command handler chunks and stores
var parentId = Guid.NewGuid();
var chunks = chunkingService.Chunk(dietSummary);  // Split into 2-3 chunks

for (int i = 0; i < chunks.Count; i++)
{
    var embedding = await embeddingService.GenerateEmbeddingAsync(chunks[i]);

    var doc = ConversationDocument.Create(
        parentDocumentId: parentId,
        chunkIndex: i,
        conversationId: conversationId,
        title: "Diet Summary - Last 7 Days",
        content: chunks[i],
        embedding: embedding,
        source: DocumentSource.NutritionData
    );

    await repository.AddAsync(doc);
}

// 4. When user asks question, semantic search finds relevant chunks
var questionEmbedding = await embeddingService.GenerateEmbeddingAsync("Am I eating enough protein?");

var relevantChunks = await context.ConversationDocuments
    .Where(x => x.ConversationId == conversationId)
    .OrderBy(x => x.Embedding.CosineDistance(questionEmbedding))
    .Take(3)
    .ToListAsync();

// 5. Include in system prompt
var systemPrompt = $"""
    You are a nutrition assistant.

    User's Diet Data:
    {string.Join("\n\n", relevantChunks.Select(x => x.Content))}

    Answer the user's question based on this data.
    """;
```

---

## Conclusion

For MealMind, **Approach 1 (Logical Grouping)** is the right choice. It's simpler, faster, and fits the use case perfectly. Save Approach 2 for future features if you add document upload functionality.
