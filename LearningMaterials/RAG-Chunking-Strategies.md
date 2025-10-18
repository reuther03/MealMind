# RAG Chunking Strategies

This document explains different strategies for chunking documents in a RAG (Retrieval-Augmented Generation) system and how to avoid common pitfalls.

---

## The Problem

When you split documents into chunks, you risk splitting related information:

```
Document: "Protein helps build muscle. A typical adult needs 0.8-2.2g per kg of body weight..."

❌ Bad Chunking (blindly split at 500 tokens):
Chunk 1: "...protein helps build muscle. A typical adult needs"
Chunk 2: "0.8-2.2g per kg of body weight. Children require..."

Problem: The context about "what adults need" is split across two chunks!
```

When a user asks "How much protein do adults need?", semantic search might find Chunk 2 but miss the context from Chunk 1.

---

## Solution 1: Add Overlap Between Chunks ✅

**Concept:** Make chunks overlap so information at boundaries appears in multiple chunks.

### Implementation

```csharp
var chunks = TextChunker.SplitPlainTextParagraphs(
    paragraphs: paragraphs,
    maxTokensPerParagraph: 500,
    overlapTokens: 50  // Last 50 tokens of chunk N appear in chunk N+1
);
```

### How It Works

```
Document: "Protein helps build muscle. A typical adult needs 0.8-2.2g per kg..."

With 50 token overlap:
Chunk 1: "Protein helps build muscle. A typical adult needs 0.8-2.2g..."
Chunk 2: "...A typical adult needs 0.8-2.2g per kg. Children require..."
         ↑ This part appears in both chunks
```

### Recommendations

- **Overlap size:** 10-20% of chunk size
  - 500 token chunks → 50-100 token overlap
  - 800 token chunks → 80-160 token overlap
  - 1000 token chunks → 100-200 token overlap

### Pros
✅ Simple to implement
✅ Prevents context loss at boundaries
✅ Works with any chunking strategy

### Cons
❌ Storage duplication (same text in multiple chunks)
❌ Doesn't solve the problem of splitting mid-topic

---

## Solution 2: Semantic Chunking (Respect Natural Boundaries) ✅ RECOMMENDED

**Concept:** Split at logical boundaries (sections, paragraphs) instead of arbitrary token counts.

### Priority Order for Splitting

1. **Document sections** (e.g., `## Protein`, `## Carbohydrates`)
2. **Subsections** (e.g., `### Daily Requirements`)
3. **Paragraphs** (double newline `\n\n`)
4. **Sentences** (periods)
5. **Tokens** (last resort)

### Implementation

```csharp
public List<string> ChunkDocument(string content, int maxTokensPerChunk = 800, int overlapTokens = 100)
{
    // Split by markdown sections (## Header)
    var sections = SplitBySections(content);

    var chunks = new List<string>();

    foreach (var section in sections)
    {
        // If section is small enough, keep it whole
        if (EstimateTokens(section) <= maxTokensPerChunk)
        {
            chunks.Add(section);  // ✅ Keep entire section together
        }
        else
        {
            // Section too large, split by paragraphs with overlap
            var paragraphs = section.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);

            var subChunks = TextChunker.SplitPlainTextParagraphs(
                paragraphs: paragraphs,
                maxTokensPerParagraph: maxTokensPerChunk,
                overlapTokens: overlapTokens
            );

            chunks.AddRange(subChunks);
        }
    }

    return chunks;
}

private List<string> SplitBySections(string content)
{
    // Split by markdown headers: ## Section
    var sections = new List<string>();
    var lines = content.Split('\n');
    var currentSection = new StringBuilder();

    foreach (var line in lines)
    {
        // Start new section when we hit a header
        if (line.StartsWith("## ") && currentSection.Length > 0)
        {
            sections.Add(currentSection.ToString());
            currentSection.Clear();
        }
        currentSection.AppendLine(line);
    }

    if (currentSection.Length > 0)
        sections.Add(currentSection.ToString());

    return sections;
}

private int EstimateTokens(string text)
{
    // Rough estimate: 1 token ≈ 4 characters
    return text.Length / 4;
}
```

### Example

**Input Document:**
```markdown
## Protein Guide

Protein is essential for muscle growth and repair.

### Daily Requirements
Adults need 0.8-2.2g per kg of body weight.
Athletes need 1.6-2.2g per kg.

## Carbohydrate Guide

Carbs provide energy for your body.

### Daily Requirements
Adults need 45-65% of calories from carbs.
```

**Output Chunks:**
```
Chunk 0 (Section 1):
## Protein Guide

Protein is essential for muscle growth and repair.

### Daily Requirements
Adults need 0.8-2.2g per kg of body weight.
Athletes need 1.6-2.2g per kg.

Chunk 1 (Section 2):
## Carbohydrate Guide

Carbs provide energy for your body.

### Daily Requirements
Adults need 45-65% of calories from carbs.
```

**Result:** Each complete section stays together! ✅

### Pros
✅ Keeps related information together
✅ Respects document structure
✅ Better retrieval quality
✅ Users get complete answers

### Cons
❌ Variable chunk sizes (some chunks may be very small or large)
❌ Requires structured documents (markdown, headers)

---

## Solution 3: Add Metadata to Chunks

**Concept:** Store which section/topic each chunk belongs to.

### Database Schema

```csharp
public class RagDocument : Document
{
    public string SectionTitle { get; private set; }  // "Protein Guide - Daily Requirements"
    public string DocumentTitle { get; private set; } // "Complete Nutrition Guide"

    public static RagDocument Create(
        Guid documentGroupId,
        int chunkIndex,
        string documentTitle,
        string content,
        Vector? embedding,
        string sectionTitle = null)
    {
        return new RagDocument
        {
            Id = Guid.NewGuid(),
            DocumentGroupId = documentGroupId,
            ChunkIndex = chunkIndex,
            DocumentTitle = documentTitle,
            Content = content,
            Embedding = embedding,
            SectionTitle = sectionTitle ?? documentTitle,
            AttachedAt = DateTime.UtcNow
        };
    }
}
```

### Usage

```csharp
var parentId = Guid.NewGuid();
var sections = SplitBySections(document);

foreach (var (sectionTitle, sectionContent, index) in sections)
{
    var chunks = ChunkSection(sectionContent);

    foreach (var (chunkText, chunkIndex) in chunks)
    {
        var embedding = await _embeddingService.GenerateEmbeddingAsync(chunkText);

        var doc = RagDocument.Create(
            documentGroupId: parentId,
            chunkIndex: index,
            documentTitle: "Complete Nutrition Guide",
            content: chunkText,
            embedding: embedding,
            sectionTitle: sectionTitle  // "Protein Guide - Daily Requirements"
        );

        await _repository.AddAsync(doc);
    }
}
```

### Benefits

**When retrieving:**
```csharp
var results = await SearchRelevantChunks(userQuestion);

foreach (var chunk in results)
{
    Console.WriteLine($"Source: {chunk.SectionTitle}");
    Console.WriteLine($"Content: {chunk.Content}");
}

// Output:
// Source: Protein Guide - Daily Requirements
// Content: Adults need 0.8-2.2g per kg of body weight...
```

Users know **where** the information came from!

### Pros
✅ Better context in search results
✅ Users can see source section
✅ Can filter by section (e.g., "only search protein-related chunks")

### Cons
❌ More complex database schema
❌ Requires parsing document structure

---

## Solution 4: Use Larger Chunks

**Concept:** Instead of 500 tokens, use 800-1000 token chunks to preserve more context.

### Configuration

```csharp
// Before (small chunks):
var chunks = ChunkDocument(content, maxTokensPerChunk: 500, overlapTokens: 50);

// After (larger chunks):
var chunks = ChunkDocument(content, maxTokensPerChunk: 1000, overlapTokens: 150);
```

### When to Use

- ✅ Documents have complex, interconnected information
- ✅ Cost/speed is less important than quality
- ✅ You have a large context window in your LLM (e.g., 8k+ tokens)

### Trade-offs

| Chunk Size | Pros | Cons |
|------------|------|------|
| **Small (300-500 tokens)** | Fast search, cheap LLM calls, precise matching | Loses context, may split topics |
| **Medium (500-800 tokens)** | Balanced | Balanced |
| **Large (800-1000+ tokens)** | Better context, complete topics | Slower search, expensive LLM calls, less precise |

### Recommendations

- **Short docs (FAQs, tips):** 300-500 tokens
- **Medium docs (guides, articles):** 500-800 tokens ← **MealMind**
- **Long docs (books, research papers):** 800-1000+ tokens

---

## Solution 5: Hierarchical Chunking (Advanced)

**Concept:** Create parent chunks (large) and child chunks (small). Search children for precision, include parent for context.

### Database Schema

```csharp
public class RagDocument : Document
{
    public Guid? ParentChunkId { get; private set; }  // NULL for parent, set for children
    public bool IsParent => ParentChunkId == null;
}
```

### Example

```
Parent Chunk (1000 tokens):
## Protein Guide
Protein is essential for muscle growth...
### Daily Requirements
Adults need 0.8-2.2g per kg...
### Best Sources
Chicken, fish, eggs, beans...

Child Chunks (300 tokens each):
├─ Child 1: "Protein is essential for muscle growth..."
├─ Child 2: "Adults need 0.8-2.2g per kg of body weight..."
└─ Child 3: "Best sources include chicken, fish, eggs..."
```

### Search Flow

1. **Search child chunks** for precise matches (smaller = better semantic matching)
2. **Find matching child:** "Adults need 0.8-2.2g per kg..."
3. **Include parent chunk** in LLM prompt for full context

### Implementation

```csharp
// 1. Create parent chunk
var parentEmbedding = await _embeddingService.GenerateEmbeddingAsync(fullSection);
var parent = RagDocument.Create(
    documentGroupId: parentGroupId,
    chunkIndex: 0,
    title: "Protein Guide",
    content: fullSection,  // Full section (1000 tokens)
    embedding: parentEmbedding,
    parentChunkId: null  // This is the parent
);
await _repository.AddAsync(parent);

// 2. Create child chunks
var childChunks = ChunkDocument(fullSection, maxTokensPerChunk: 300);
foreach (var (childContent, index) in childChunks)
{
    var childEmbedding = await _embeddingService.GenerateEmbeddingAsync(childContent);
    var child = RagDocument.Create(
        documentGroupId: parentGroupId,
        chunkIndex: index + 1,
        title: "Protein Guide",
        content: childContent,
        embedding: childEmbedding,
        parentChunkId: parent.Id  // Link to parent
    );
    await _repository.AddAsync(child);
}

// 3. Search and retrieve
var matchingChildren = await SearchChildChunks(userQuestion);
var parentsToInclude = await _context.RagDocuments
    .Where(x => matchingChildren.Select(c => c.ParentChunkId).Contains(x.Id))
    .ToListAsync();

// 4. Include both in system prompt
var systemPrompt = $"""
    Relevant sections:
    {string.Join("\n\n", parentsToInclude.Select(p => p.Content))}

    Specific details:
    {string.Join("\n\n", matchingChildren.Select(c => c.Content))}
    """;
```

### Pros
✅ Best of both worlds: precise search + full context
✅ Flexible retrieval strategy

### Cons
❌ Complex implementation
❌ 2x storage (parent + children)
❌ More embeddings to generate

---

## Solution 6: Sentence-Window Retrieval

**Concept:** Embed individual sentences, but return surrounding sentences for context.

### How It Works

```
Document split into sentences:
[S1, S2, S3, S4, S5, S6, S7, S8, S9, S10]

User asks question → matches S6

Return to LLM (window of 5 sentences):
[S4, S5, S6, S7, S8]
      ↑ Match ↑
```

### Implementation

```csharp
// 1. Split into sentences
var sentences = document.Split(new[] { ". ", "! ", "? " }, StringSplitOptions.RemoveEmptyEntries);

// 2. Embed each sentence
for (int i = 0; i < sentences.Length; i++)
{
    var embedding = await _embeddingService.GenerateEmbeddingAsync(sentences[i]);

    var doc = RagDocument.Create(
        documentGroupId: parentId,
        chunkIndex: i,
        title: documentTitle,
        content: sentences[i],
        embedding: embedding
    );

    await _repository.AddAsync(doc);
}

// 3. Search for matching sentence
var matchingSentence = await SearchBestSentence(userQuestion);
var matchIndex = matchingSentence.ChunkIndex;

// 4. Retrieve window around match
var windowSize = 2;  // 2 before + match + 2 after = 5 sentences
var contextSentences = await _context.RagDocuments
    .Where(x => x.DocumentGroupId == matchingSentence.DocumentGroupId)
    .Where(x => x.ChunkIndex >= matchIndex - windowSize && x.ChunkIndex <= matchIndex + windowSize)
    .OrderBy(x => x.ChunkIndex)
    .ToListAsync();

var context = string.Join(". ", contextSentences.Select(s => s.Content));
```

### Pros
✅ Very precise matching (sentence-level)
✅ Flexible context window
✅ No overlap needed

### Cons
❌ Many small embeddings to generate/store
❌ Loses paragraph-level structure
❌ Sentence splitting can be tricky (abbreviations, etc.)

---

## Comparison Table

| Strategy | Complexity | Context Preservation | Storage Cost | Best For |
|----------|------------|---------------------|--------------|----------|
| **Overlap** | Low | Medium | Medium | All use cases |
| **Semantic Chunking** | Medium | High | Low | Structured documents |
| **Metadata** | Medium | High | Low | Better UX |
| **Larger Chunks** | Low | High | Low | When cost isn't an issue |
| **Hierarchical** | High | Very High | High | Complex documents |
| **Sentence-Window** | High | High | High | Very precise search needed |

---

## Recommended Strategy for MealMind

**Combine Semantic Chunking + Overlap + Metadata:**

```csharp
public class ImprovedChunkingService : IChunkingService
{
    public List<ChunkResult> ChunkDocument(
        string content,
        string documentTitle,
        int maxTokensPerChunk = 800,
        int overlapTokens = 100)
    {
        var results = new List<ChunkResult>();
        var sections = SplitBySections(content);

        foreach (var (sectionTitle, sectionContent) in sections)
        {
            if (EstimateTokens(sectionContent) <= maxTokensPerChunk)
            {
                // Keep small sections intact
                results.Add(new ChunkResult
                {
                    Content = sectionContent,
                    SectionTitle = sectionTitle,
                    DocumentTitle = documentTitle
                });
            }
            else
            {
                // Split large sections by paragraphs with overlap
                var paragraphs = sectionContent.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
                var chunks = TextChunker.SplitPlainTextParagraphs(
                    paragraphs: paragraphs,
                    maxTokensPerParagraph: maxTokensPerChunk,
                    overlapTokens: overlapTokens
                );

                foreach (var chunk in chunks)
                {
                    results.Add(new ChunkResult
                    {
                        Content = chunk,
                        SectionTitle = sectionTitle,
                        DocumentTitle = documentTitle
                    });
                }
            }
        }

        return results;
    }
}

public class ChunkResult
{
    public string Content { get; set; }
    public string SectionTitle { get; set; }
    public string DocumentTitle { get; set; }
}
```

**Why this works for MealMind:**
- ✅ Nutrition/workout guides have clear sections (## Protein, ## Carbs)
- ✅ Keeps related info together (protein requirements in one chunk)
- ✅ Metadata helps users understand context
- ✅ 800 token chunks = good balance of context vs. cost
- ✅ 100 token overlap prevents boundary issues

---

## Testing Your Chunking Strategy

Create a test document and verify chunks make sense:

```csharp
[Fact]
public async Task ChunkingPreservesTopicBoundaries()
{
    var document = """
        ## Protein Guide

        Protein is essential for muscle growth.

        ### Daily Requirements
        Adults need 0.8-2.2g per kg of body weight.

        ## Carbohydrate Guide

        Carbs provide energy.

        ### Daily Requirements
        Adults need 45-65% of calories from carbs.
        """;

    var chunks = _chunkingService.ChunkDocument(document, "Nutrition Guide");

    // Should create 2 chunks (one per section)
    Assert.Equal(2, chunks.Count);

    // First chunk should contain protein info
    Assert.Contains("Protein", chunks[0].Content);
    Assert.Contains("0.8-2.2g", chunks[0].Content);
    Assert.DoesNotContain("Carbs", chunks[0].Content);

    // Second chunk should contain carb info
    Assert.Contains("Carbs", chunks[1].Content);
    Assert.Contains("45-65%", chunks[1].Content);
    Assert.DoesNotContain("Protein", chunks[1].Content);
}
```

---

## Token Estimation

Since you're working with Ollama models, you can estimate tokens:

```csharp
private int EstimateTokens(string text)
{
    // Rough estimate for English text:
    // 1 token ≈ 4 characters (or 0.75 words)
    return text.Length / 4;
}
```

For precise token counting, use a tokenizer library:
```bash
dotnet add package Microsoft.ML.Tokenizers
```

```csharp
using Microsoft.ML.Tokenizers;

private int CountTokens(string text)
{
    var tokenizer = Tokenizer.CreateTiktokenForModel("gpt-4");
    return tokenizer.CountTokens(text);
}
```

---

## Summary

**Start simple, iterate based on results:**

1. **Phase 1 (MVP):** Fixed-size chunks (500 tokens) + overlap (50 tokens)
2. **Phase 2:** Semantic chunking (respect sections) + larger chunks (800 tokens)
3. **Phase 3:** Add metadata (section titles)
4. **Phase 4:** Hierarchical chunking (if needed for very complex docs)

**Most important:** Test with real queries and see if the retrieved chunks actually answer the questions! Adjust chunk size and overlap based on results.
