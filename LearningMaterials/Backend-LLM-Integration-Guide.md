# Backend LLM Integration - Advanced Topics

A comprehensive guide to working with Large Language Models in production backend applications, based on real-world experience from the MealMind project.

---

## Table of Contents
1. [Model Selection & Evaluation](#1-model-selection--evaluation)
2. [Prompt Engineering for Production](#2-prompt-engineering-for-production)
3. [Structured Output & JSON Mode](#3-structured-output--json-mode)
4. [Context Management & Token Optimization](#4-context-management--token-optimization)
5. [Error Handling & Resilience](#5-error-handling--resilience)
6. [Performance & Latency](#6-performance--latency)
7. [RAG System Design](#7-rag-system-design)
8. [Cost Optimization](#8-cost-optimization)
9. [Security & Safety](#9-security--safety)
10. [Monitoring & Observability](#10-monitoring--observability)
11. [Testing LLM Applications](#11-testing-llm-applications)
12. [Advanced Patterns](#12-advanced-patterns)

---

## 1. Model Selection & Evaluation

### Choosing the Right Model

**Key Considerations:**
- **Task complexity** - Simple Q&A vs complex reasoning
- **Latency requirements** - Real-time (<2s) vs batch processing
- **Infrastructure** - Cloud API vs self-hosted
- **Cost** - Per-token pricing vs GPU costs
- **Privacy** - Data sensitivity and compliance

**Model Size vs Quality Trade-off:**
```
┌─────────────────────────────────────────────────────┐
│                                                     │
│  Small (3-4B)     Medium (7-8B)     Large (70B+)   │
│  ────────────     ─────────────     ─────────────   │
│  Fast (1-3s)      Moderate (3-8s)   Slow (10-30s)  │
│  Lower quality    Good quality      Best quality   │
│  Cheap            Moderate cost     Expensive      │
│  GPU optional     GPU recommended   GPU required   │
│                                                     │
└─────────────────────────────────────────────────────┘
```

**Real Example from MealMind:**
```
llama3.2:3b          →  qwen3:4b
─────────────────────────────────────────────────
❌ Hallucinated sources    ✅ Accurate citations
❌ Malformed JSON          ✅ Valid JSON 99%+
❌ Dummy responses         ✅ Consistent quality
❌ 50%+ retry rate         ✅ <5% retry rate
```

**Lesson Learned:** Don't assume "newest" or "most popular" = best for your use case. Test multiple models with your actual prompts and data.

### Evaluation Metrics

**1. Functional Metrics:**
- **Accuracy**: Does it answer correctly?
- **Hallucination rate**: Does it invent facts?
- **Format compliance**: Does it follow JSON schema?
- **Source attribution**: Does it cite correctly?

**2. Performance Metrics:**
- **Latency (P50, P95, P99)**: Response time distribution
- **Throughput**: Requests per second
- **Token efficiency**: Output tokens per request
- **First token time**: Time to start streaming

**3. Business Metrics:**
- **Cost per request**: API fees or GPU time
- **User satisfaction**: Ratings, feedback
- **Retry rate**: How often do responses fail?
- **Conversion rate**: Does it help users achieve goals?

### Testing Framework

```csharp
public class LlmModelEvaluator
{
    // Test suite of representative queries
    private readonly List<EvalQuery> _testQueries = new()
    {
        new("How much protein for muscle gain?", ExpectedSources: ["Basic Nutrition Guidelines"]),
        new("Best sources of vitamin D?", ExpectedSources: ["Micronutrients Overview"]),
        // ... 20-50 test cases
    };

    public async Task<ModelScores> EvaluateModel(string modelName)
    {
        var scores = new ModelScores();

        foreach (var query in _testQueries)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await _llmClient.GetResponse(query.Prompt, modelName);
            stopwatch.Stop();

            // Score accuracy (manual or LLM-as-judge)
            scores.Accuracy += ScoreAccuracy(response, query.ExpectedAnswer);

            // Check hallucination
            scores.HallucinationRate += CheckHallucination(response, query.ExpectedSources);

            // Check format
            scores.FormatCompliance += response.IsValidJson ? 1 : 0;

            // Track latency
            scores.LatencyP95 = CalculatePercentile(scores.Latencies, 0.95);
        }

        return scores;
    }
}
```

---

## 2. Prompt Engineering for Production

### Principle: Clarity > Brevity

**Bad (Development):**
```
Answer the user's question using the documents below.
```

**Good (Production):**
```
You are a nutrition knowledge assistant. Answer questions **using ONLY** the reference documents below.

REFERENCE DOCUMENTS:
{documents}

TASK:
Answer the user's question by extracting factual, numeric details from the documents.
Return ONLY valid JSON matching this schema: {...}

EXAMPLE OUTPUT:
{concrete example}

RESTRICTIONS:
- Do NOT invent information
- Do NOT include markdown code fences
- First character must be '{'
```

### Key Techniques

**1. Show, Don't Tell**
- ✅ Provide concrete examples of desired output
- ❌ Don't just describe what you want

**2. Negative Examples**
- ✅ Show what NOT to do
- Prevents common failure modes

**3. Format Enforcement**
```
RESTRICTIONS:
- Do NOT output anything except JSON
- No ```json fences or markdown
- First character must be '{' and last character must be '}'
```

**4. Schema Definition**
```json
{
  "Title": "string - max 100 chars",
  "Paragraphs": ["array of 2-5 strings", "each 100-250 words"],
  "KeyPoints": ["array of 3-7 strings", "each 10-30 words"],
  "Sources": ["document titles only"]
}
```

**5. Pre-flight Checklist**
```
Before finishing, check:
1. JSON is syntactically valid
2. All fields match schema
3. Every paragraph uses actual data from documents
```

### Temperature Settings

```
Task Type              | Temperature | Use Case
──────────────────────────────────────────────────────
Factual Q&A            | 0.0 - 0.2   | RAG, knowledge retrieval
Structured Output      | 0.1 - 0.3   | JSON generation, forms
Creative Writing       | 0.7 - 0.9   | Blog posts, storytelling
Code Generation        | 0.2 - 0.4   | Boilerplate, templates
Brainstorming          | 0.8 - 1.2   | Ideas, alternatives
```

**MealMind uses 0.2** for consistent, factual responses.

### System Prompt Organization

**Structure:**
```
1. Role definition
2. Context/documents
3. Task description
4. Output schema
5. Concrete examples (positive + negative)
6. Detailed requirements
7. Restrictions (what NOT to do)
8. Pre-flight checklist
```

---

## 3. Structured Output & JSON Mode

### Why Structured Output Matters

**Without JSON Schema Enforcement:**
```json
// LLM might return:
```json
{
  "title": "Protein Info",
  "content": "..."
}
```  ← markdown fence wrapping!
```

**With JSON Schema Enforcement:**
```csharp
ChatResponseFormat.ForJsonSchema<StructuredResponse>()
```
→ Guaranteed valid JSON matching your C# class

### Best Practices

**1. Define DTOs with Clear Names**
```csharp
public record StructuredResponse(
    string Title,              // Clear and descriptive
    List<string> Paragraphs,   // Not "Content" or "Body"
    List<string> KeyPoints,    // Specific purpose
    List<string> Sources       // Domain-specific
);
```

**2. Validate Before Deserialize**
```csharp
// Fast check before expensive deserialization
if (!response.Text.StartsWith('{') || !response.Text.EndsWith('}'))
{
    return await RepairJson(response.Text);
}

var result = JsonSerializer.Deserialize<StructuredResponse>(response.Text)!;
```

**3. Handle Edge Cases**
```csharp
// Missing information response
{
  "Title": "Information Not Available",
  "Paragraphs": ["I don't have that information in my knowledge base."],
  "KeyPoints": [],
  "Sources": []
}
```

### Advanced: JSON Repair Pattern

```csharp
public async Task<T> GetStructuredResponse<T>(string prompt)
{
    var response = await _llm.GetResponse(prompt);

    // Validate format
    if (!IsValidJsonFormat(response.Text))
    {
        // Attempt repair with stricter prompt
        response = await _llm.GetResponse(
            $"Fix this JSON to match schema:\n{response.Text}",
            responseFormat: ChatResponseFormat.ForJsonSchema<T>()
        );
    }

    return JsonSerializer.Deserialize<T>(response.Text)!;
}
```

---

## 4. Context Management & Token Optimization

### Understanding Token Limits

```
┌──────────────────────────────────────────────────┐
│  Model Context Window                            │
│  ┌────────────────────────────────────────────┐  │
│  │  System Prompt (500-2000 tokens)           │  │
│  │  ────────────────────────────────────────  │  │
│  │  Retrieved Documents (1000-4000 tokens)    │  │
│  │  ────────────────────────────────────────  │  │
│  │  Conversation History (0-2000 tokens)      │  │
│  │  ────────────────────────────────────────  │  │
│  │  User Query (10-200 tokens)                │  │
│  │  ────────────────────────────────────────  │  │
│  │  [Reserved for Output] (500-2000 tokens)   │  │
│  └────────────────────────────────────────────┘  │
│                                                   │
│  Common Limits:                                   │
│  - Llama 3.2: 128K tokens                         │
│  - Qwen 3: 32K tokens                             │
│  - GPT-4: 128K tokens                             │
└──────────────────────────────────────────────────┘
```

### Token Optimization Strategies

**1. Chunk Retrieved Documents**
```csharp
// Instead of sending full documents
var relevantChunks = await _documentRepo.GetTopChunks(
    query: userQuestion,
    topK: 5,  // Only top 5 most relevant chunks
    maxTokens: 2000
);
```

**2. Summarize Long Contexts**
```csharp
if (conversationHistory.TokenCount > 2000)
{
    // Summarize older messages
    var summary = await _llm.Summarize(conversationHistory.Take(10));
    conversationHistory = new[] { summary }.Concat(conversationHistory.TakeLast(5));
}
```

**3. Sliding Window for Chat History**
```csharp
public class ConversationManager
{
    private const int MaxMessages = 10;

    public List<ChatMessage> GetContextWindow(List<ChatMessage> allMessages)
    {
        // Keep system prompt + last N messages
        var systemPrompt = allMessages.First();
        var recentMessages = allMessages.TakeLast(MaxMessages);

        return new[] { systemPrompt }.Concat(recentMessages).ToList();
    }
}
```

**4. Smart Document Filtering**
```csharp
// Filter documents by relevance threshold
var relevantDocs = await _vectorDb.Search(query, threshold: 0.7);

// Re-rank by importance
var reranked = relevantDocs
    .OrderByDescending(doc => doc.Recency * 0.3 + doc.Relevance * 0.7)
    .Take(3);
```

---

## 5. Error Handling & Resilience

### Common Failure Modes

**1. Network Timeouts**
```csharp
var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

try
{
    var response = await _llm.GetResponse(prompt, cts.Token);
}
catch (OperationCanceledException)
{
    _logger.LogWarning("LLM request timed out after 30s");
    return FallbackResponse();
}
```

**2. Rate Limiting**
```csharp
public class RateLimitedLlmClient
{
    private readonly SemaphoreSlim _semaphore = new(maxConcurrent: 5);

    public async Task<string> GetResponse(string prompt)
    {
        await _semaphore.WaitAsync();
        try
        {
            return await _llm.GetResponse(prompt);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
```

**3. Malformed Responses**
```csharp
public async Task<StructuredResponse> GetStructuredResponse(string prompt)
{
    var response = await _llm.GetResponse(prompt);

    // Validation
    if (!IsValidFormat(response))
    {
        _logger.LogWarning("Malformed response, attempting repair");
        response = await RepairResponse(response);
    }

    // Deserialization with error handling
    try
    {
        return JsonSerializer.Deserialize<StructuredResponse>(response)!;
    }
    catch (JsonException ex)
    {
        _logger.LogError(ex, "Failed to deserialize LLM response");
        return GetFallbackResponse();
    }
}
```

### Retry Strategies

**Exponential Backoff with Polly:**
```csharp
services.AddHttpClient<ILlmClient, OllamaClient>()
    .AddTransientHttpErrorPolicy(builder => builder
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
            onRetry: (outcome, timespan, retryCount, context) =>
            {
                _logger.LogWarning($"Retry {retryCount} after {timespan.TotalSeconds}s");
            }
        ));
```

**Smart Retry (Different Prompts):**
```csharp
public async Task<T> GetResponseWithRetry<T>(string prompt, int maxAttempts = 3)
{
    for (int attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            var response = await _llm.GetResponse(
                prompt: attempt == 1 ? prompt : MakeStricterPrompt(prompt),
                temperature: Math.Max(0.1f, 0.3f - (attempt * 0.1f)) // Lower temp on retry
            );

            return ParseResponse<T>(response);
        }
        catch (Exception ex) when (attempt < maxAttempts)
        {
            _logger.LogWarning($"Attempt {attempt} failed: {ex.Message}");
            await Task.Delay(TimeSpan.FromSeconds(attempt));
        }
    }

    throw new LlmException("All retry attempts failed");
}
```

### Circuit Breaker Pattern

```csharp
public class LlmCircuitBreaker
{
    private int _failureCount = 0;
    private DateTime _lastFailure = DateTime.MinValue;
    private const int FailureThreshold = 5;
    private readonly TimeSpan _resetTimeout = TimeSpan.FromMinutes(5);

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        // Check if circuit is open
        if (_failureCount >= FailureThreshold)
        {
            if (DateTime.UtcNow - _lastFailure < _resetTimeout)
            {
                throw new CircuitBreakerOpenException("LLM service temporarily unavailable");
            }

            // Half-open state - try again
            _failureCount = 0;
        }

        try
        {
            var result = await action();
            _failureCount = 0; // Reset on success
            return result;
        }
        catch
        {
            _failureCount++;
            _lastFailure = DateTime.UtcNow;
            throw;
        }
    }
}
```

---

## 6. Performance & Latency

### Measurement Points

```
┌─────────────────────────────────────────────────┐
│  User Request                                   │
│  ↓                                              │
│  [1] Query Embedding (50-200ms)                 │
│  ↓                                              │
│  [2] Vector Search (10-100ms)                   │
│  ↓                                              │
│  [3] Document Retrieval (5-50ms)                │
│  ↓                                              │
│  [4] Prompt Assembly (1-10ms)                   │
│  ↓                                              │
│  [5] LLM Inference (1000-5000ms) ← BOTTLENECK   │
│  ↓                                              │
│  [6] Response Parsing (5-20ms)                  │
│  ↓                                              │
│  User Response                                  │
└─────────────────────────────────────────────────┘
```

### Optimization Techniques

**1. Streaming Responses**
```csharp
public async IAsyncEnumerable<string> StreamResponse(string prompt)
{
    await foreach (var chunk in _llm.StreamAsync(prompt))
    {
        yield return chunk; // Send to client immediately
    }
}
```

**2. Parallel Processing**
```csharp
// Generate embedding and retrieve conversation history in parallel
var embeddingTask = _embeddingService.GenerateAsync(userQuery);
var historyTask = _conversationRepo.GetHistoryAsync(conversationId);

await Task.WhenAll(embeddingTask, historyTask);

var embedding = await embeddingTask;
var history = await historyTask;
```

**3. Caching**
```csharp
public class CachedLlmClient : ILlmClient
{
    private readonly IMemoryCache _cache;
    private readonly ILlmClient _innerClient;

    public async Task<string> GetResponse(string prompt)
    {
        var cacheKey = $"llm:{ComputeHash(prompt)}";

        if (_cache.TryGetValue(cacheKey, out string cached))
        {
            return cached;
        }

        var response = await _innerClient.GetResponse(prompt);

        _cache.Set(cacheKey, response, TimeSpan.FromHours(1));

        return response;
    }
}
```

**4. GPU Optimization**
```yaml
# docker-compose.yml
services:
  ollama:
    image: ollama/ollama:latest
    deploy:
      resources:
        reservations:
          devices:
            - driver: nvidia
              count: 1
              capabilities: [gpu]
    environment:
      - OLLAMA_NUM_GPU=1          # Use 1 GPU
      - OLLAMA_NUM_PARALLEL=4     # Handle 4 requests in parallel
      - OLLAMA_MAX_LOADED_MODELS=2 # Keep 2 models in VRAM
```

### Benchmarking

```csharp
public class LlmBenchmark
{
    [Benchmark]
    public async Task<string> SmallModel_Qwen3_4B()
    {
        return await _llm.GetResponse(TestPrompt, model: "qwen3:4b");
    }

    [Benchmark]
    public async Task<string> MediumModel_Llama3_8B()
    {
        return await _llm.GetResponse(TestPrompt, model: "llama3.1:8b");
    }

    [Benchmark]
    public async Task<string> WithCache()
    {
        return await _cachedLlm.GetResponse(TestPrompt);
    }
}
```

---

## 7. RAG System Design

### Architecture Patterns

**Pattern 1: Simple RAG (MealMind Approach)**
```
User Query → Embedding → Vector Search → Top K Docs → LLM → Response
```

**Pattern 2: Advanced RAG**
```
User Query → Query Expansion (LLM generates variations)
           ↓
    Multi-Query Search (search each variation)
           ↓
    Hybrid Search (keyword + vector)
           ↓
    Re-ranking (score by relevance)
           ↓
    Context Compression (remove irrelevant sentences)
           ↓
    LLM with Compressed Context
           ↓
    Response + Citations
```

### Key Design Decisions

**1. Chunk Size**
```
Small Chunks (200-300 tokens)
✅ More precise retrieval
✅ Less noise in context
❌ May miss broader context
❌ More chunks to manage

Large Chunks (800-1200 tokens)
✅ Better context preservation
✅ Fewer database entries
❌ More noise in retrieval
❌ Higher token usage
```

**MealMind uses ~500 tokens** as a sweet spot.

**2. Embedding Model Selection**
```
Model                  | Dimensions | Speed    | Quality
────────────────────────────────────────────────────────
all-MiniLM-L6         | 384        | Fastest  | Basic
nomic-embed-text      | 768        | Fast     | Good
qwen3-embedding:0.6b  | 1024       | Fast     | Better
text-embedding-3-large| 3072       | Moderate | Best
```

**3. Similarity Metrics**
```sql
-- Cosine Similarity (most common)
SELECT * FROM documents
ORDER BY embedding <=> query_embedding
LIMIT 5;

-- L2 Distance
SELECT * FROM documents
ORDER BY embedding <-> query_embedding
LIMIT 5;

-- Inner Product
SELECT * FROM documents
ORDER BY embedding <#> query_embedding DESC
LIMIT 5;
```

**MealMind uses cosine (`<=>`)** - good for normalized embeddings.

### Advanced Techniques

**1. Hybrid Search (Keyword + Semantic)**
```csharp
public async Task<List<Document>> HybridSearch(string query)
{
    // Semantic search
    var semanticResults = await _vectorDb.Search(query, topK: 10);

    // Keyword search (BM25)
    var keywordResults = await _fullTextSearch.Search(query, topK: 10);

    // Merge with RRF (Reciprocal Rank Fusion)
    return MergeResults(semanticResults, keywordResults);
}

private List<Document> MergeResults(List<Document> list1, List<Document> list2)
{
    var scores = new Dictionary<Guid, double>();

    foreach (var (doc, rank) in list1.Select((doc, i) => (doc, i)))
    {
        scores[doc.Id] = scores.GetValueOrDefault(doc.Id) + 1.0 / (rank + 60);
    }

    foreach (var (doc, rank) in list2.Select((doc, i) => (doc, i)))
    {
        scores[doc.Id] = scores.GetValueOrDefault(doc.Id) + 1.0 / (rank + 60);
    }

    return scores
        .OrderByDescending(kvp => kvp.Value)
        .Take(5)
        .Select(kvp => list1.Concat(list2).First(d => d.Id == kvp.Key))
        .ToList();
}
```

**2. Query Expansion**
```csharp
public async Task<List<string>> ExpandQuery(string originalQuery)
{
    var prompt = $"""
        Generate 3 alternative phrasings of this question:
        "{originalQuery}"

        Return as JSON array: ["variant1", "variant2", "variant3"]
        """;

    var response = await _llm.GetResponse(prompt);
    return JsonSerializer.Deserialize<List<string>>(response)!;
}
```

**3. Re-ranking**
```csharp
public async Task<List<Document>> RerankDocuments(
    string query,
    List<Document> candidates)
{
    var scores = new List<(Document doc, double score)>();

    foreach (var doc in candidates)
    {
        // Ask LLM to score relevance 0-10
        var prompt = $"""
            On a scale of 0-10, how relevant is this document to the query?
            Query: {query}
            Document: {doc.Content}
            Return only a number.
            """;

        var scoreText = await _llm.GetResponse(prompt, temperature: 0.0f);
        var score = double.Parse(scoreText);

        scores.Add((doc, score));
    }

    return scores
        .OrderByDescending(x => x.score)
        .Select(x => x.doc)
        .ToList();
}
```

---

## 8. Cost Optimization

### Cloud LLM Pricing (Approximate)

```
Provider    | Model          | Input ($/1M) | Output ($/1M)
──────────────────────────────────────────────────────────
OpenAI      | GPT-4 Turbo    | $10          | $30
OpenAI      | GPT-3.5 Turbo  | $0.50        | $1.50
Anthropic   | Claude 3 Opus  | $15          | $75
Anthropic   | Claude 3 Haiku | $0.25        | $1.25
Google      | Gemini Pro     | $0.50        | $1.50
```

### Self-Hosted Cost Analysis

**GPU Server Costs:**
```
Hardware              | Upfront    | Monthly Power | Notes
────────────────────────────────────────────────────────────
RTX 4090 (24GB)       | $1,600     | ~$50          | 7B-13B models
RTX 4080 (16GB)       | $1,200     | ~$40          | 3B-7B models
Cloud GPU (A100)      | $0         | ~$1,000       | Pay-per-hour
```

**Break-even Analysis:**
```
Scenario: 10,000 requests/month, 1000 tokens avg

Cloud (GPT-3.5):
10,000 * 1,000 tokens / 1M * $1.00 = $10/month

Self-hosted (RTX 4090):
Hardware: $1,600 / 24 months = $67/month
Power: $50/month
Total: $117/month

Break-even at ~120,000 requests/month
```

### Token Optimization Strategies

**1. Prompt Compression**
```csharp
// Before: 200 tokens
var verbosePrompt = """
    You are an assistant that helps users with their questions.
    Please answer the following question using the context provided.
    Make sure to be accurate and cite your sources.

    Context: {context}
    Question: {question}
    """;

// After: 50 tokens
var compressedPrompt = """
    Answer using only this context: {context}
    Q: {question}
    """;
```

**2. Smart Context Pruning**
```csharp
public string PruneContext(string context, int maxTokens)
{
    var sentences = context.Split('.');
    var tokenCount = 0;
    var result = new List<string>();

    foreach (var sentence in sentences)
    {
        var sentenceTokens = EstimateTokens(sentence);
        if (tokenCount + sentenceTokens > maxTokens) break;

        result.Add(sentence);
        tokenCount += sentenceTokens;
    }

    return string.Join('. ', result);
}
```

**3. Batch Processing**
```csharp
// Instead of 100 individual requests
foreach (var query in queries)
{
    await _llm.GetResponse(query); // 100 API calls
}

// Batch into one request
var batchPrompt = $"""
    Answer these questions:
    1. {queries[0]}
    2. {queries[1]}
    ...

    Return as JSON array.
    """;
var responses = await _llm.GetResponse(batchPrompt); // 1 API call
```

---

## 9. Security & Safety

### Input Validation

**1. Prompt Injection Defense**
```csharp
public class PromptInjectionDetector
{
    private readonly string[] _suspiciousPatterns = new[]
    {
        "ignore previous instructions",
        "ignore all previous",
        "disregard previous",
        "forget previous",
        "new instructions:",
        "system:",
        "assistant:",
        // Add more patterns
    };

    public bool IsSuspicious(string userInput)
    {
        return _suspiciousPatterns.Any(pattern =>
            userInput.Contains(pattern, StringComparison.OrdinalIgnoreCase));
    }

    public string Sanitize(string userInput)
    {
        // Remove potential instruction delimiters
        return userInput
            .Replace("###", "")
            .Replace("---", "")
            .Replace("```", "");
    }
}
```

**2. Content Filtering**
```csharp
public async Task<bool> IsContentSafe(string text)
{
    // Use OpenAI Moderation API or similar
    var response = await _moderationClient.ModerateAsync(text);

    return !response.Flagged;
}
```

### Output Validation

**1. PII Detection**
```csharp
public class PiiDetector
{
    private readonly Regex _emailRegex = new(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b");
    private readonly Regex _phoneRegex = new(@"\b\d{3}[-.]?\d{3}[-.]?\d{4}\b");
    private readonly Regex _ssnRegex = new(@"\b\d{3}-\d{2}-\d{4}\b");

    public string RedactPii(string text)
    {
        text = _emailRegex.Replace(text, "[EMAIL]");
        text = _phoneRegex.Replace(text, "[PHONE]");
        text = _ssnRegex.Replace(text, "[SSN]");

        return text;
    }
}
```

**2. Hallucination Detection**
```csharp
public bool ValidateSources(StructuredResponse response, List<string> availableSources)
{
    foreach (var source in response.Sources)
    {
        if (!availableSources.Contains(source))
        {
            _logger.LogWarning($"LLM hallucinated source: {source}");
            return false;
        }
    }

    return true;
}
```

### Access Control

```csharp
public class LlmAccessControl
{
    private readonly IUserRepository _userRepo;

    public async Task<bool> CanAccessLlm(Guid userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);

        // Check quota
        var usage = await GetMonthlyUsage(userId);
        if (usage > user.LlmQuota)
        {
            return false;
        }

        // Check permissions
        return user.HasPermission("llm:query");
    }

    public async Task<int> GetMonthlyUsage(Guid userId)
    {
        var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        return await _db.LlmRequests
            .Where(r => r.UserId == userId && r.CreatedAt >= startOfMonth)
            .CountAsync();
    }
}
```

---

## 10. Monitoring & Observability

### Key Metrics to Track

**1. Request Metrics**
```csharp
public class LlmMetrics
{
    private readonly IMetricsCollector _metrics;

    public async Task<string> GetResponseWithMetrics(string prompt)
    {
        var stopwatch = Stopwatch.StartNew();
        var tokenCount = EstimateTokens(prompt);

        try
        {
            var response = await _llm.GetResponse(prompt);

            _metrics.RecordHistogram("llm.latency_ms", stopwatch.ElapsedMilliseconds);
            _metrics.RecordHistogram("llm.input_tokens", tokenCount);
            _metrics.RecordHistogram("llm.output_tokens", EstimateTokens(response));
            _metrics.IncrementCounter("llm.requests.success");

            return response;
        }
        catch (Exception ex)
        {
            _metrics.IncrementCounter($"llm.requests.error.{ex.GetType().Name}");
            throw;
        }
    }
}
```

**2. Quality Metrics**
```csharp
public async Task TrackResponseQuality(StructuredResponse response, string query)
{
    // Track hallucination rate
    var hasHallucination = !ValidateSources(response);
    _metrics.RecordGauge("llm.hallucination_rate", hasHallucination ? 1 : 0);

    // Track format compliance
    var isValidFormat = response.Paragraphs.Count >= 2 && response.KeyPoints.Count >= 3;
    _metrics.RecordGauge("llm.format_compliance", isValidFormat ? 1 : 0);

    // Track retry rate
    if (response.WasRepaired)
    {
        _metrics.IncrementCounter("llm.responses.repaired");
    }
}
```

**3. Cost Tracking**
```csharp
public async Task<decimal> CalculateRequestCost(LlmRequest request)
{
    var inputCost = request.InputTokens / 1_000_000.0m * _pricing.InputPricePerMillion;
    var outputCost = request.OutputTokens / 1_000_000.0m * _pricing.OutputPricePerMillion;

    var totalCost = inputCost + outputCost;

    await _db.LlmCosts.AddAsync(new LlmCost
    {
        RequestId = request.Id,
        InputTokens = request.InputTokens,
        OutputTokens = request.OutputTokens,
        Cost = totalCost,
        Timestamp = DateTime.UtcNow
    });

    _metrics.RecordHistogram("llm.cost_usd", (double)totalCost);

    return totalCost;
}
```

### Logging Best Practices

```csharp
public class LlmLogger
{
    private readonly ILogger<LlmLogger> _logger;

    public async Task<string> GetResponseWithLogging(string prompt, Guid userId)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["UserId"] = userId,
            ["PromptLength"] = prompt.Length,
            ["Timestamp"] = DateTime.UtcNow
        });

        _logger.LogInformation("LLM request started");

        try
        {
            var response = await _llm.GetResponse(prompt);

            _logger.LogInformation("LLM request completed", new
            {
                ResponseLength = response.Length,
                Success = true
            });

            // Log full prompt/response to separate storage (e.g., blob storage)
            await _auditLog.LogLlmInteraction(userId, prompt, response);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LLM request failed");
            throw;
        }
    }
}
```

### Alerting

```csharp
public class LlmHealthMonitor
{
    public async Task MonitorHealth()
    {
        var metrics = await GetLast5MinutesMetrics();

        // Alert on high error rate
        if (metrics.ErrorRate > 0.1) // 10%
        {
            await _alerting.SendAlert("LLM Error Rate High",
                $"Error rate: {metrics.ErrorRate:P}");
        }

        // Alert on high latency
        if (metrics.LatencyP95 > TimeSpan.FromSeconds(10))
        {
            await _alerting.SendAlert("LLM Latency High",
                $"P95 latency: {metrics.LatencyP95.TotalSeconds}s");
        }

        // Alert on hallucination spike
        if (metrics.HallucinationRate > 0.05) // 5%
        {
            await _alerting.SendAlert("LLM Hallucination Rate High",
                $"Hallucination rate: {metrics.HallucinationRate:P}");
        }
    }
}
```

---

## 11. Testing LLM Applications

### Unit Testing

**1. Mock LLM Responses**
```csharp
public class FakeLlmClient : ILlmClient
{
    private readonly Dictionary<string, string> _responses = new()
    {
        ["How much protein for muscle gain?"] = """
        {
            "Title": "Protein Requirements",
            "Paragraphs": ["1.6-2.2 g/kg body weight..."],
            "KeyPoints": ["1.6-2.2 g/kg daily"],
            "Sources": ["Basic Nutrition Guidelines"]
        }
        """
    };

    public Task<string> GetResponse(string prompt)
    {
        // Simple substring matching for test prompts
        var match = _responses.FirstOrDefault(kvp => prompt.Contains(kvp.Key));
        return Task.FromResult(match.Value ?? GetDefaultResponse());
    }
}

[Fact]
public async Task ResponseManager_ShouldParseValidJson()
{
    // Arrange
    var fakeLlm = new FakeLlmClient();
    var responseManager = new ResponseManager(fakeLlm);

    // Act
    var result = await responseManager.GenerateStructuredResponseAsync(
        "How much protein for muscle gain?",
        documents: "...",
        documentTitles: new[] { "Basic Nutrition Guidelines" }
    );

    // Assert
    Assert.NotEmpty(result.Paragraphs);
    Assert.Contains("Basic Nutrition Guidelines", result.Sources);
}
```

**2. Test Prompt Templates**
```csharp
[Theory]
[InlineData("protein", "Protein Requirements")]
[InlineData("vitamins", "Micronutrients")]
public async Task PromptBuilder_ShouldIncludeRelevantDocs(string query, string expectedDoc)
{
    var prompt = _promptBuilder.BuildPrompt(query, relevantDocs);

    Assert.Contains(expectedDoc, prompt);
}
```

### Integration Testing

**1. Test Against Real LLM (Optional)**
```csharp
[Fact]
[Trait("Category", "Integration")]
public async Task RealLlm_ShouldReturnValidJson()
{
    // Only run in CI/staging, not on every commit
    var llm = new OllamaClient("http://localhost:11434");

    var response = await llm.GetResponse(
        prompt: "Return JSON with title field",
        model: "qwen3:4b"
    );

    var parsed = JsonSerializer.Deserialize<dynamic>(response);
    Assert.NotNull(parsed);
}
```

**2. Test RAG Pipeline End-to-End**
```csharp
[Fact]
public async Task RagPipeline_ShouldRetrieveAndGenerate()
{
    // Arrange: Seed test documents
    await SeedTestDocuments();

    // Act: Query the system
    var result = await _ragService.GetAnswer("How much protein?");

    // Assert
    Assert.NotEmpty(result.Paragraphs);
    Assert.Contains("1.6-2.2", result.Paragraphs[0]); // Factual number
    Assert.NotEmpty(result.Sources);
}
```

### Evaluation Testing

**1. Golden Dataset**
```csharp
public class LlmEvaluationTests
{
    private readonly List<GoldenExample> _goldenDataset = new()
    {
        new("How much protein?", ExpectedKeywords: ["1.6-2.2", "g/kg"]),
        new("Best vitamin D sources?", ExpectedKeywords: ["sunlight", "fish", "fortified"]),
        // ... 50-100 examples
    };

    [Fact]
    public async Task Evaluate_Qwen3_OnGoldenDataset()
    {
        var scores = new List<double>();

        foreach (var example in _goldenDataset)
        {
            var response = await _llm.GetResponse(example.Query, model: "qwen3:4b");

            var score = ScoreResponse(response, example.ExpectedKeywords);
            scores.Add(score);
        }

        var avgScore = scores.Average();

        Assert.True(avgScore > 0.8, $"Average score {avgScore} is below threshold");
    }

    private double ScoreResponse(string response, List<string> keywords)
    {
        var matches = keywords.Count(kw =>
            response.Contains(kw, StringComparison.OrdinalIgnoreCase));

        return (double)matches / keywords.Count;
    }
}
```

**2. A/B Testing Framework**
```csharp
public class ModelComparisonTest
{
    [Fact]
    public async Task CompareModels_Qwen3_vs_Llama3()
    {
        var testQueries = GetTestQueries();

        var qwen3Scores = await EvaluateModel("qwen3:4b", testQueries);
        var llama3Scores = await EvaluateModel("llama3.2:3b", testQueries);

        _output.WriteLine($"Qwen3 Avg: {qwen3Scores.Average()}");
        _output.WriteLine($"Llama3 Avg: {llama3Scores.Average()}");

        Assert.True(qwen3Scores.Average() > llama3Scores.Average());
    }
}
```

---

## 12. Advanced Patterns

### 1. Multi-Agent Systems

**Concept:** Multiple LLMs with specialized roles collaborate.

```csharp
public class MultiAgentSystem
{
    private readonly ILlmClient _researcher;  // Finds information
    private readonly ILlmClient _writer;      // Generates response
    private readonly ILlmClient _critic;      // Reviews accuracy

    public async Task<StructuredResponse> GetAnswer(string query)
    {
        // Agent 1: Research
        var researchPrompt = $"Find key facts about: {query}";
        var facts = await _researcher.GetResponse(researchPrompt);

        // Agent 2: Write
        var writePrompt = $"Write a response using these facts: {facts}";
        var draft = await _writer.GetResponse(writePrompt);

        // Agent 3: Critique
        var critiquePrompt = $"Review this response for accuracy: {draft}";
        var critique = await _critic.GetResponse(critiquePrompt);

        // Revise if needed
        if (critique.Contains("inaccurate"))
        {
            draft = await _writer.GetResponse($"Revise based on: {critique}");
        }

        return ParseResponse(draft);
    }
}
```

### 2. Chain-of-Thought (CoT)

**Concept:** Make the LLM "think step-by-step" before answering.

```csharp
public async Task<string> GetAnswerWithCoT(string query)
{
    var cotPrompt = $"""
        Question: {query}

        Think step-by-step:
        1. What information do I need?
        2. What documents are relevant?
        3. What are the key facts?
        4. What's the final answer?

        Return your reasoning as JSON:
        {{
            "reasoning": ["step 1", "step 2", ...],
            "answer": "final answer"
        }}
        """;

    var response = await _llm.GetResponse(cotPrompt);
    var parsed = JsonSerializer.Deserialize<CoTResponse>(response);

    // Log reasoning for debugging
    _logger.LogInformation("CoT Reasoning: {Reasoning}",
        string.Join(" → ", parsed.Reasoning));

    return parsed.Answer;
}
```

### 3. Self-Reflection

**Concept:** LLM evaluates and improves its own output.

```csharp
public async Task<StructuredResponse> GetAnswerWithReflection(string query)
{
    // Initial response
    var draft = await _llm.GetResponse(query);

    // Self-reflection
    var reflectionPrompt = $"""
        Review this response and identify any issues:
        {draft}

        Check for:
        1. Factual accuracy
        2. Completeness
        3. Clarity

        Return JSON:
        {{
            "hasIssues": true/false,
            "issues": ["issue 1", "issue 2"],
            "improvement": "improved response if needed"
        }}
        """;

    var reflection = await _llm.GetResponse(reflectionPrompt);
    var parsed = JsonSerializer.Deserialize<ReflectionResponse>(reflection);

    // Use improved version if issues found
    return parsed.HasIssues
        ? ParseResponse(parsed.Improvement)
        : ParseResponse(draft);
}
```

### 4. Retrieval-Augmented Fine-Tuning (RAFT)

**Concept:** Fine-tune the model on domain-specific Q&A with RAG.

```csharp
// 1. Generate training data from your RAG system
public async Task<List<TrainingExample>> GenerateTrainingData()
{
    var examples = new List<TrainingExample>();

    foreach (var query in _commonQueries)
    {
        // Get RAG answer
        var context = await _vectorDb.Search(query);
        var answer = await _llm.GetResponse(BuildPrompt(query, context));

        examples.Add(new TrainingExample
        {
            Instruction = query,
            Context = context,
            Output = answer
        });
    }

    return examples;
}

// 2. Fine-tune (using external service or tools like Ollama)
// 3. Use fine-tuned model in production
```

### 5. Hybrid LLM Strategy

**Concept:** Use different models for different tasks.

```csharp
public class HybridLlmRouter
{
    private readonly ILlmClient _fastModel;   // qwen3:4b
    private readonly ILlmClient _smartModel;  // llama3.1:8b

    public async Task<string> GetResponse(string query, QueryType type)
    {
        return type switch
        {
            QueryType.Simple => await _fastModel.GetResponse(query),
            QueryType.Complex => await _smartModel.GetResponse(query),
            QueryType.Factual => await _fastModel.GetResponse(query),
            QueryType.Reasoning => await _smartModel.GetResponse(query),
            _ => throw new ArgumentException()
        };
    }

    private QueryType ClassifyQuery(string query)
    {
        // Simple heuristic or use small classifier model
        if (query.Contains("calculate") || query.Contains("analyze"))
            return QueryType.Complex;

        return QueryType.Simple;
    }
}
```

### 6. Streaming with Server-Sent Events (SSE)

**Concept:** Stream LLM responses to frontend in real-time.

```csharp
[HttpGet("chat-stream")]
public async Task StreamChat(string query)
{
    Response.ContentType = "text/event-stream";

    await foreach (var chunk in _llm.StreamResponse(query))
    {
        await Response.WriteAsync($"data: {chunk}\n\n");
        await Response.Body.FlushAsync();
    }
}
```

**Frontend:**
```typescript
const eventSource = new EventSource('/chat-stream?query=protein');

eventSource.onmessage = (event) => {
    const chunk = event.data;
    appendToUI(chunk);
};
```

---

## Summary & Next Steps

### Key Takeaways

1. **Model Selection Matters** - Test multiple models with your actual use case
2. **Prompt Engineering is Critical** - Clear, structured prompts with examples
3. **Structured Output** - Use JSON schema enforcement for reliability
4. **Error Handling** - Validate, retry, and have fallbacks
5. **Monitor Everything** - Track latency, quality, cost, errors
6. **Optimize Iteratively** - Start simple, measure, then optimize
7. **Security First** - Validate inputs, sanitize outputs, control access

### Learning Path

**Beginner (Weeks 1-4):**
- ✅ Basic LLM integration (Ollama/OpenAI)
- ✅ Prompt engineering fundamentals
- ✅ JSON structured output
- ✅ Simple RAG implementation

**Intermediate (Weeks 5-12):**
- ⬜ Advanced prompt patterns (CoT, few-shot)
- ⬜ Vector databases and embeddings
- ⬜ Chunking strategies
- ⬜ Error handling and retries
- ⬜ Monitoring and metrics

**Advanced (Months 4-6):**
- ⬜ Multi-agent systems
- ⬜ Hybrid search (semantic + keyword)
- ⬜ Query expansion and re-ranking
- ⬜ Fine-tuning for domain
- ⬜ A/B testing frameworks
- ⬜ Custom evaluation metrics

### Recommended Resources

**Books:**
- "Building LLM Applications" by Valentina Alto
- "Designing Machine Learning Systems" by Chip Huyen
- "Patterns of Enterprise Application Architecture" by Martin Fowler

**Courses:**
- DeepLearning.AI - "LangChain for LLM Application Development"
- Andrew Ng - "Building Systems with the ChatGPT API"
- Fast.ai - "Practical Deep Learning"

**Papers:**
- "Retrieval-Augmented Generation" (Lewis et al., 2020)
- "Chain-of-Thought Prompting" (Wei et al., 2022)
- "Constitutional AI" (Anthropic, 2022)

**Tools & Frameworks:**
- **LangChain** - Python/JS framework for LLM apps
- **Semantic Kernel** - Microsoft's .NET LLM framework
- **LlamaIndex** - Data framework for LLM apps
- **LangSmith** - LLM observability platform
- **Weights & Biases** - Experiment tracking

### Practice Projects

1. **Build a RAG Chatbot** (like MealMind)
2. **LLM-Powered Code Review Tool**
3. **Automated Customer Support System**
4. **Document Q&A System**
5. **SQL Query Generator from Natural Language**

---

## Conclusion

Working with LLMs in backend applications is part **software engineering**, part **machine learning**, and part **experimentation**. The field evolves rapidly, so stay curious, measure everything, and iterate based on real-world results.

Your MealMind project demonstrates many of these principles in practice - from model selection (Llama → Qwen3), to structured output, to RAG implementation. Keep building! 🚀

---

**Last Updated:** 2025-10-24
**Author:** Michał Drozd (MealMind Project)
