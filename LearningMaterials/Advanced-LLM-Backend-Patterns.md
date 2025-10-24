# Advanced LLM Backend Patterns - Beyond RAG

A deep-dive into essential LLM integration patterns for production backend systems. This guide covers function calling, agents, embeddings, fine-tuning, and modern architectural patterns.

---

## Table of Contents

### Core Concepts
1. [Function Calling & Tool Use](#1-function-calling--tool-use)
2. [LLM Agents & Orchestration](#2-llm-agents--orchestration)
3. [Embeddings Deep Dive](#3-embeddings-deep-dive)
4. [Fine-Tuning for Domain Adaptation](#4-fine-tuning-for-domain-adaptation)
5. [Long-Context Processing](#5-long-context-processing)

### Advanced Patterns
6. [Semantic Caching](#6-semantic-caching)
7. [LLM-Powered Backend Features](#7-llm-powered-backend-features)
8. [Guardrails & Content Moderation](#8-guardrails--content-moderation)
9. [Multi-Modal LLM Integration](#9-multi-modal-llm-integration)
10. [Batch Processing & Background Jobs](#10-batch-processing--background-jobs)

### Production Engineering
11. [LLM API Gateway Pattern](#11-llm-api-gateway-pattern)
12. [State Management for Conversations](#12-state-management-for-conversations)
13. [Prompt Versioning & A/B Testing](#13-prompt-versioning--ab-testing)
14. [LLM-Driven Workflows](#14-llm-driven-workflows)
15. [Real-Time Streaming & WebSockets](#15-real-time-streaming--websockets)

---

## 1. Function Calling & Tool Use

### What is Function Calling?

**Concept:** LLM decides *when* and *how* to call backend functions based on user input.

**Use Cases:**
- Database queries ("Show me users from last week")
- External API calls ("What's the weather in Warsaw?")
- Business logic ("Transfer $100 to account X")
- Calculations ("Calculate my BMI")

### Basic Implementation

```csharp
// 1. Define available tools/functions
public class NutritionTools
{
    [Description("Calculate daily calorie needs based on user data")]
    public async Task<int> CalculateTDEE(
        [Description("Weight in kg")] double weight,
        [Description("Height in cm")] double height,
        [Description("Age in years")] int age,
        [Description("Activity level: sedentary, light, moderate, very_active")] string activityLevel)
    {
        // Harris-Benedict equation
        var bmr = 10 * weight + 6.25 * height - 5 * age + 5;

        var multiplier = activityLevel switch
        {
            "sedentary" => 1.2,
            "light" => 1.375,
            "moderate" => 1.55,
            "very_active" => 1.725,
            _ => 1.2
        };

        return (int)(bmr * multiplier);
    }

    [Description("Get user's current nutrition profile")]
    public async Task<UserProfile> GetUserProfile(
        [Description("User ID")] Guid userId)
    {
        return await _userRepo.GetProfileAsync(userId);
    }

    [Description("Create a meal plan for given calorie target")]
    public async Task<MealPlan> CreateMealPlan(
        [Description("Target calories")] int calories,
        [Description("Number of meals per day")] int mealsPerDay,
        [Description("Dietary restrictions (comma separated)")] string restrictions)
    {
        // Generate meal plan logic
        return new MealPlan { /* ... */ };
    }
}
```

### LLM Function Calling Flow

```csharp
public class FunctionCallingService
{
    private readonly IChatClient _chatClient;
    private readonly NutritionTools _tools;

    public async Task<string> ProcessUserQuery(string userQuery, Guid userId)
    {
        // 1. Define available functions for LLM
        var toolDefinitions = new[]
        {
            new ChatTool
            {
                Name = "calculate_tdee",
                Description = "Calculate daily calorie needs",
                Parameters = new
                {
                    type = "object",
                    properties = new
                    {
                        weight = new { type = "number", description = "Weight in kg" },
                        height = new { type = "number", description = "Height in cm" },
                        age = new { type = "integer", description = "Age in years" },
                        activityLevel = new { type = "string", enum = new[] { "sedentary", "light", "moderate", "very_active" } }
                    },
                    required = new[] { "weight", "height", "age", "activityLevel" }
                }
            },
            new ChatTool
            {
                Name = "get_user_profile",
                Description = "Get user's current nutrition profile",
                Parameters = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "string", format = "uuid" }
                    },
                    required = new[] { "userId" }
                }
            }
        };

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, "You are a nutrition assistant. Use available tools to help users."),
            new(ChatRole.User, userQuery)
        };

        // 2. LLM decides which function to call
        var response = await _chatClient.GetResponseAsync(messages, new ChatOptions
        {
            Tools = toolDefinitions,
            Temperature = 0.1f
        });

        // 3. Check if LLM wants to call a function
        if (response.ToolCalls?.Count > 0)
        {
            foreach (var toolCall in response.ToolCalls)
            {
                // 4. Execute the function
                var result = await ExecuteTool(toolCall);

                // 5. Send function result back to LLM
                messages.Add(new ChatMessage(ChatRole.Tool, result)
                {
                    ToolCallId = toolCall.Id
                });
            }

            // 6. Get final response from LLM
            var finalResponse = await _chatClient.GetResponseAsync(messages);
            return finalResponse.Text;
        }

        return response.Text;
    }

    private async Task<string> ExecuteTool(ToolCall toolCall)
    {
        var args = JsonSerializer.Deserialize<Dictionary<string, object>>(toolCall.Arguments);

        return toolCall.Name switch
        {
            "calculate_tdee" => JsonSerializer.Serialize(await _tools.CalculateTDEE(
                (double)args["weight"],
                (double)args["height"],
                (int)args["age"],
                (string)args["activityLevel"]
            )),
            "get_user_profile" => JsonSerializer.Serialize(await _tools.GetUserProfile(
                Guid.Parse((string)args["userId"])
            )),
            _ => throw new NotImplementedException($"Tool {toolCall.Name} not found")
        };
    }
}
```

### Real-World Example

```
User: "I'm 80kg, 180cm, 30 years old and moderately active. What's my daily calorie target?"

LLM thinks:
  → Need to calculate TDEE
  → I have a function for this!

LLM calls:
  calculate_tdee(weight: 80, height: 180, age: 30, activityLevel: "moderate")

Backend executes:
  → Returns: 2,480 calories

LLM responds:
  "Based on your stats, your daily calorie target is 2,480 calories.
   This accounts for your moderate activity level."
```

### Advanced: Multi-Step Tool Use

```csharp
// User: "Create a meal plan for me"
// LLM orchestrates multiple tools:

public async Task<string> MultiStepWorkflow(string userQuery, Guid userId)
{
    var messages = new List<ChatMessage>
    {
        new(ChatRole.System, "You are a nutrition assistant. Use tools to complete complex tasks."),
        new(ChatRole.User, userQuery)
    };

    // Step 1: LLM calls get_user_profile
    var response1 = await _chatClient.GetResponseAsync(messages, tools: _toolDefinitions);
    // → get_user_profile(userId: "123")
    // → Returns: { weight: 80, height: 180, age: 30, activityLevel: "moderate", goal: "muscle_gain" }

    messages.Add(new ChatMessage(ChatRole.Tool, userProfileJson));

    // Step 2: LLM calls calculate_tdee
    var response2 = await _chatClient.GetResponseAsync(messages, tools: _toolDefinitions);
    // → calculate_tdee(weight: 80, height: 180, age: 30, activityLevel: "moderate")
    // → Returns: 2480

    messages.Add(new ChatMessage(ChatRole.Tool, "2480"));

    // Step 3: LLM calls create_meal_plan
    var response3 = await _chatClient.GetResponseAsync(messages, tools: _toolDefinitions);
    // → create_meal_plan(calories: 2480, mealsPerDay: 4, restrictions: "")

    // Step 4: LLM generates final response
    var final = await _chatClient.GetResponseAsync(messages);
    // → "I've created a 2,480 calorie meal plan with 4 meals per day..."

    return final.Text;
}
```

### Function Calling Best Practices

**1. Clear Function Descriptions**
```csharp
// ❌ Bad
[Description("Get data")]
public Task<User> GetUser(Guid id) { }

// ✅ Good
[Description("Retrieve user's nutrition profile including weight, height, goals, and dietary restrictions")]
public Task<UserNutritionProfile> GetUserNutritionProfile(
    [Description("The unique identifier of the user")] Guid userId
) { }
```

**2. Validate Function Calls**
```csharp
private async Task<string> ExecuteTool(ToolCall toolCall)
{
    // Validate before execution
    if (!_allowedTools.Contains(toolCall.Name))
    {
        throw new UnauthorizedAccessException($"Tool {toolCall.Name} not allowed");
    }

    // Validate arguments
    var args = JsonSerializer.Deserialize<Dictionary<string, object>>(toolCall.Arguments);
    if (toolCall.Name == "transfer_money" && (decimal)args["amount"] > 1000)
    {
        throw new InvalidOperationException("Amount exceeds limit");
    }

    // Execute
    return await InvokeTool(toolCall.Name, args);
}
```

**3. Handle Errors Gracefully**
```csharp
private async Task<string> ExecuteTool(ToolCall toolCall)
{
    try
    {
        return await InvokeTool(toolCall.Name, toolCall.Arguments);
    }
    catch (Exception ex)
    {
        // Return error to LLM so it can handle it
        return JsonSerializer.Serialize(new
        {
            error = true,
            message = ex.Message,
            suggestion = "Please try a different approach or ask for clarification"
        });
    }
}
```

---

## 2. LLM Agents & Orchestration

### What are LLM Agents?

**Agent:** An LLM that can:
1. Plan actions to achieve a goal
2. Use tools/functions
3. Observe results
4. Adjust its plan
5. Iterate until goal is reached

**Types of Agents:**

```
┌─────────────────────────────────────────────────────────┐
│  Agent Type        │ Complexity │ Use Case              │
├────────────────────┼────────────┼───────────────────────┤
│  ReAct Agent       │ Medium     │ General Q&A with tools│
│  Plan-and-Execute  │ High       │ Complex workflows     │
│  Reflexion Agent   │ High       │ Self-improving tasks  │
│  AutoGPT-style     │ Very High  │ Autonomous goals      │
└─────────────────────────────────────────────────────────┘
```

### ReAct Agent Implementation

**ReAct = Reasoning + Acting**

```csharp
public class ReActAgent
{
    private readonly IChatClient _llm;
    private readonly Dictionary<string, Func<string, Task<string>>> _tools;

    public async Task<string> RunAgent(string goal, int maxIterations = 5)
    {
        var thoughts = new List<string>();
        var actions = new List<string>();
        var observations = new List<string>();

        for (int i = 0; i < maxIterations; i++)
        {
            // 1. THINK: LLM reasons about what to do next
            var thinkPrompt = $$$"""
                Goal: {{{goal}}}

                Previous thoughts: {{{string.Join("\n", thoughts)}}}
                Previous actions: {{{string.Join("\n", actions)}}}
                Previous observations: {{{string.Join("\n", observations)}}}

                Available tools:
                - search_documents(query): Search nutrition knowledge base
                - get_user_data(userId): Get user profile
                - calculate_macros(calories, goal): Calculate macro split

                Think step-by-step about what to do next.
                Return JSON:
                {
                    "thought": "your reasoning",
                    "action": "tool_name",
                    "action_input": "input for tool",
                    "is_final": false
                }

                If you have enough information to answer, set is_final: true and provide "final_answer".
                """;

            var thinkResponse = await _llm.GetResponseAsync(thinkPrompt);
            var decision = JsonSerializer.Deserialize<AgentDecision>(thinkResponse.Text);

            thoughts.Add(decision.Thought);

            // 2. CHECK: Is this the final answer?
            if (decision.IsFinal)
            {
                return decision.FinalAnswer;
            }

            // 3. ACT: Execute the chosen tool
            actions.Add($"{decision.Action}({decision.ActionInput})");
            var observation = await _tools[decision.Action](decision.ActionInput);
            observations.Add(observation);
        }

        return "Max iterations reached. Could not complete the goal.";
    }
}

public record AgentDecision(
    string Thought,
    string Action,
    string ActionInput,
    bool IsFinal,
    string? FinalAnswer = null
);
```

### Real Example: Nutrition Plan Agent

```csharp
public class NutritionPlanAgent
{
    public async Task<string> CreatePersonalizedPlan(Guid userId)
    {
        var goal = $"Create a personalized nutrition plan for user {userId}";

        var agent = new ReActAgent(_llm, new Dictionary<string, Func<string, Task<string>>>
        {
            ["get_user_data"] = async (input) =>
            {
                var user = await _userRepo.GetByIdAsync(Guid.Parse(input));
                return JsonSerializer.Serialize(new
                {
                    user.Weight,
                    user.Height,
                    user.Age,
                    user.Goal,
                    user.DietaryRestrictions
                });
            },
            ["calculate_tdee"] = async (input) =>
            {
                var args = JsonSerializer.Deserialize<TDEEArgs>(input);
                var tdee = await _nutritionService.CalculateTDEE(args);
                return tdee.ToString();
            },
            ["search_meal_database"] = async (input) =>
            {
                var meals = await _mealRepo.SearchByCaloriesAsync(int.Parse(input));
                return JsonSerializer.Serialize(meals);
            },
            ["save_nutrition_plan"] = async (input) =>
            {
                var plan = JsonSerializer.Deserialize<NutritionPlan>(input);
                await _planRepo.SaveAsync(plan);
                return "Plan saved successfully";
            }
        });

        return await agent.RunAgent(goal);
    }
}
```

**Agent Execution Trace:**
```
Iteration 1:
  Thought: "I need to get user data first"
  Action: get_user_data("user-guid")
  Observation: { weight: 80, height: 180, goal: "muscle_gain", ... }

Iteration 2:
  Thought: "I need to calculate their calorie needs"
  Action: calculate_tdee({ weight: 80, height: 180, ... })
  Observation: "2800"

Iteration 3:
  Thought: "I should find meals that fit this calorie target"
  Action: search_meal_database("2800")
  Observation: [{ meal: "Breakfast", calories: 700, ... }, ...]

Iteration 4:
  Thought: "I have all the information to create a plan"
  Action: save_nutrition_plan({ userId: "...", meals: [...], totalCalories: 2800 })
  Observation: "Plan saved successfully"

Iteration 5:
  Thought: "Plan is created and saved"
  Final Answer: "I've created a 2,800 calorie muscle gain plan with 4 meals per day..."
```

### Plan-and-Execute Agent

**More sophisticated: separate planning from execution**

```csharp
public class PlanAndExecuteAgent
{
    public async Task<string> RunAgent(string goal)
    {
        // 1. PLANNING PHASE: Create high-level plan
        var planPrompt = $"""
            Goal: {goal}

            Create a step-by-step plan to achieve this goal.
            Return as JSON array:
            [
                {{ "step": 1, "action": "description", "tool": "tool_name" }},
                {{ "step": 2, "action": "description", "tool": "tool_name" }}
            ]
            """;

        var planResponse = await _llm.GetResponseAsync(planPrompt);
        var plan = JsonSerializer.Deserialize<List<PlanStep>>(planResponse.Text);

        // 2. EXECUTION PHASE: Execute each step
        var results = new List<string>();

        foreach (var step in plan)
        {
            var result = await ExecuteStep(step, results);
            results.Add(result);

            // 3. RE-PLANNING: Adjust if needed
            if (StepFailed(result))
            {
                plan = await ReplanFromHere(goal, step, results);
            }
        }

        // 4. SYNTHESIS: Combine results into final answer
        return await SynthesizeAnswer(goal, results);
    }

    private async Task<List<PlanStep>> ReplanFromHere(string goal, PlanStep failedStep, List<string> priorResults)
    {
        var replanPrompt = $"""
            Original goal: {goal}

            We completed these steps:
            {string.Join("\n", priorResults)}

            But this step failed: {failedStep.Action}

            Create a new plan from here to complete the goal.
            """;

        var newPlan = await _llm.GetResponseAsync(replanPrompt);
        return JsonSerializer.Deserialize<List<PlanStep>>(newPlan.Text);
    }
}
```

### Agent Patterns in MealMind

**Use Case: Smart Food Logger**

```csharp
public class FoodLogAgent
{
    // User: "I just ate two slices of pizza and a coke"
    // Agent automatically:
    // 1. Searches food database for "pizza"
    // 2. Estimates calories (2 slices ≈ 600 cal)
    // 3. Searches for "coke"
    // 4. Adds both to food log
    // 5. Updates daily totals
    // 6. Checks if user exceeded targets
    // 7. Provides feedback

    public async Task<FoodLogResult> LogFoodNaturalLanguage(Guid userId, string description)
    {
        var agent = new ReActAgent(_llm, new Dictionary<string, Func<string, Task<string>>>
        {
            ["search_food"] = async (query) => await _foodDbService.SearchAsync(query),
            ["estimate_portion"] = async (food) => await _nutritionService.EstimatePortion(food),
            ["add_to_log"] = async (entry) => await _foodLogRepo.AddAsync(userId, entry),
            ["get_daily_totals"] = async (_) => await _foodLogRepo.GetDailyTotalsAsync(userId),
            ["get_user_targets"] = async (_) => await _userRepo.GetNutritionTargetsAsync(userId)
        });

        return await agent.RunAgent($"Log this food and provide feedback: {description}");
    }
}
```

---

## 3. Embeddings Deep Dive

### Understanding Embeddings

**What are embeddings?**
- Vector representations of text
- Semantic meaning encoded as numbers
- Similar concepts = similar vectors

```
"protein"        → [0.23, -0.45, 0.67, ..., 0.12]  (1024 dimensions)
"muscle growth"  → [0.21, -0.43, 0.69, ..., 0.15]  ← Similar vector!
"car"            → [-0.78, 0.32, -0.12, ..., 0.89] ← Very different
```

### Embedding Use Cases Beyond RAG

**1. Semantic Search**
```csharp
public class SemanticSearchService
{
    public async Task<List<Product>> SearchProducts(string query)
    {
        // Convert query to embedding
        var queryEmbedding = await _embeddingService.GenerateAsync(query);

        // Find similar product embeddings
        var results = await _db.Products
            .OrderBy(p => p.Embedding.CosineDistance(queryEmbedding))
            .Take(10)
            .ToListAsync();

        return results;
    }
}

// User searches: "high protein breakfast"
// Finds: "eggs", "greek yogurt", "protein shake" (even without exact keyword match!)
```

**2. Duplicate Detection**
```csharp
public async Task<bool> IsDuplicateQuestion(string newQuestion, Guid conversationId)
{
    var newEmbedding = await _embeddingService.GenerateAsync(newQuestion);

    var previousQuestions = await _db.Messages
        .Where(m => m.ConversationId == conversationId && m.Role == "user")
        .ToListAsync();

    foreach (var prev in previousQuestions)
    {
        var similarity = CosineSimilarity(newEmbedding, prev.Embedding);

        if (similarity > 0.95) // 95% similar
        {
            _logger.LogInformation("Duplicate question detected: {Original} ≈ {New}", prev.Content, newQuestion);
            return true;
        }
    }

    return false;
}
```

**3. Content Classification**
```csharp
public class ContentClassifier
{
    private readonly Dictionary<string, Vector> _categoryEmbeddings;

    public ContentClassifier()
    {
        // Pre-compute category embeddings
        _categoryEmbeddings = new()
        {
            ["nutrition"] = Embed("nutrition diet food calories macros vitamins"),
            ["training"] = Embed("workout exercise gym strength cardio"),
            ["recovery"] = Embed("sleep rest recovery stress fatigue"),
            ["supplements"] = Embed("protein powder creatine vitamins supplements")
        };
    }

    public async Task<string> ClassifyUserMessage(string message)
    {
        var messageEmbedding = await _embeddingService.GenerateAsync(message);

        var scores = _categoryEmbeddings
            .Select(kvp => new
            {
                Category = kvp.Key,
                Score = CosineSimilarity(messageEmbedding, kvp.Value)
            })
            .OrderByDescending(x => x.Score)
            .ToList();

        return scores.First().Category;
    }
}

// User: "I'm feeling tired after workouts"
// Classifies as: "recovery" (even though "recovery" not mentioned explicitly)
```

**4. Recommendation System**
```csharp
public async Task<List<Article>> RecommendArticles(Guid userId)
{
    // Get user's reading history
    var readArticles = await _db.UserArticles
        .Where(ua => ua.UserId == userId)
        .Select(ua => ua.Article.Embedding)
        .ToListAsync();

    // Average embedding = user's interests
    var userInterestVector = AverageVectors(readArticles);

    // Find similar unread articles
    var recommendations = await _db.Articles
        .Where(a => !a.UserArticles.Any(ua => ua.UserId == userId))
        .OrderBy(a => a.Embedding.CosineDistance(userInterestVector))
        .Take(10)
        .ToListAsync();

    return recommendations;
}
```

**5. Clustering & Topic Extraction**
```csharp
public async Task<Dictionary<string, List<string>>> ClusterUserQuestions(Guid userId)
{
    var questions = await _db.Messages
        .Where(m => m.UserId == userId && m.Role == "user")
        .Select(m => new { m.Content, m.Embedding })
        .ToListAsync();

    // K-means clustering on embeddings
    var clusters = KMeansClustering(questions.Select(q => q.Embedding).ToList(), k: 5);

    // Extract topic for each cluster
    var topics = new Dictionary<string, List<string>>();

    for (int i = 0; i < clusters.Count; i++)
    {
        var clusterQuestions = clusters[i].Select(idx => questions[idx].Content).ToList();
        var topic = await ExtractTopic(clusterQuestions);
        topics[topic] = clusterQuestions;
    }

    return topics;

    // Result:
    // "Protein intake" → ["How much protein?", "Best protein sources?", ...]
    // "Meal timing" → ["When to eat?", "Post-workout nutrition?", ...]
}
```

### Advanced Embedding Techniques

**1. Multi-Vector Representations**
```csharp
// Store multiple embeddings for different aspects
public class RecipeEmbeddings
{
    public Guid RecipeId { get; set; }
    public Vector TitleEmbedding { get; set; }        // "High Protein Breakfast Bowl"
    public Vector IngredientsEmbedding { get; set; }  // "eggs, spinach, cheese, ..."
    public Vector NutritionEmbedding { get; set; }    // "500 cal, 30g protein, ..."
    public Vector InstructionsEmbedding { get; set; } // "Mix eggs, cook, ..."
}

public async Task<List<Recipe>> SearchRecipes(string query, SearchAspect aspect)
{
    var queryEmbedding = await _embeddingService.GenerateAsync(query);

    var results = aspect switch
    {
        SearchAspect.Title => SearchByTitleEmbedding(queryEmbedding),
        SearchAspect.Ingredients => SearchByIngredientsEmbedding(queryEmbedding),
        SearchAspect.Nutrition => SearchByNutritionEmbedding(queryEmbedding),
        SearchAspect.All => SearchByAllEmbeddings(queryEmbedding)
    };

    return await results.ToListAsync();
}
```

**2. Embedding Aggregation**
```csharp
public class ConversationSummaryService
{
    public async Task<Vector> GetConversationEmbedding(Guid conversationId)
    {
        var messages = await _db.Messages
            .Where(m => m.ConversationId == conversationId)
            .Select(m => m.Embedding)
            .ToListAsync();

        // Weighted average (recent messages have more weight)
        var weights = Enumerable.Range(1, messages.Count)
            .Select(i => Math.Pow(1.1, i)) // Exponential weight
            .ToList();

        return WeightedAverage(messages, weights);
    }

    // Use case: Find similar conversations
    public async Task<List<Guid>> FindSimilarConversations(Guid conversationId)
    {
        var embedding = await GetConversationEmbedding(conversationId);

        var similar = await _db.Conversations
            .Where(c => c.Id != conversationId)
            .OrderBy(c => c.SummaryEmbedding.CosineDistance(embedding))
            .Take(5)
            .Select(c => c.Id)
            .ToListAsync();

        return similar;
    }
}
```

**3. Cross-Lingual Embeddings**
```csharp
// Some models (like multilingual-e5) support multiple languages
public class MultilingualSearchService
{
    public async Task<List<Document>> SearchAcrossLanguages(string query, string queryLanguage)
    {
        // Embed query (works in any language)
        var queryEmbedding = await _embeddingService.GenerateAsync(query);

        // Search across documents in different languages
        var results = await _db.Documents
            .OrderBy(d => d.Embedding.CosineDistance(queryEmbedding))
            .Take(10)
            .ToListAsync();

        return results;

        // User searches in Polish: "wysokobiałkowe śniadanie"
        // Finds English docs: "high protein breakfast", "protein-rich morning meals"
    }
}
```

### Embedding Model Comparison

```
Model                    | Dims | Speed | Quality | Use Case
──────────────────────────────────────────────────────────────────
all-MiniLM-L6           | 384  | ⚡⚡⚡  | ⭐⭐     | Fast search
nomic-embed-text        | 768  | ⚡⚡    | ⭐⭐⭐   | General purpose
qwen3-embedding:0.6b    | 1024 | ⚡⚡    | ⭐⭐⭐⭐  | High quality (MealMind)
text-embedding-3-large  | 3072 | ⚡     | ⭐⭐⭐⭐⭐ | Production (OpenAI)
gte-large               | 1024 | ⚡⚡    | ⭐⭐⭐⭐  | Good balance
```

---

## 4. Fine-Tuning for Domain Adaptation

### When to Fine-Tune?

**Consider fine-tuning when:**
- ✅ Domain-specific jargon (medical, legal, technical)
- ✅ Consistent output format required
- ✅ Improved accuracy worth the effort
- ✅ Have 500+ quality examples

**Skip fine-tuning if:**
- ❌ RAG + prompt engineering works well
- ❌ Limited training data (<100 examples)
- ❌ Task changes frequently
- ❌ General model is sufficient

### Fine-Tuning Approaches

**1. Full Fine-Tuning**
- Adjust all model weights
- Requires significant compute
- Best results but expensive

**2. LoRA (Low-Rank Adaptation)**
- Only train small adapter layers
- Much faster and cheaper
- 90% of full fine-tuning quality

**3. Prompt Tuning**
- Learn optimal "soft prompts"
- Fastest approach
- Good for simple tasks

### LoRA Fine-Tuning for MealMind

**Scenario:** Fine-tune for nutrition Q&A format

```python
# Using Unsloth for fast LoRA training
from unsloth import FastLanguageModel

# 1. Load base model
model, tokenizer = FastLanguageModel.from_pretrained(
    model_name = "qwen3:4b",
    max_seq_length = 2048,
    dtype = None,
    load_in_4bit = True,
)

# 2. Prepare for LoRA
model = FastLanguageModel.get_peft_model(
    model,
    r = 16,  # LoRA rank
    target_modules = ["q_proj", "k_proj", "v_proj", "o_proj"],
    lora_alpha = 16,
    lora_dropout = 0,
    bias = "none",
)

# 3. Prepare training data
training_data = [
    {
        "instruction": "How much protein for muscle gain?",
        "context": "Basic Nutrition Guidelines: For muscle gain, consume 1.6-2.2g protein per kg body weight...",
        "output": '{"Title": "Protein for Muscle Gain", "Paragraphs": [...], "KeyPoints": [...], "Sources": [...]}'
    },
    # ... 500+ more examples
]

# 4. Train
trainer = SFTTrainer(
    model = model,
    tokenizer = tokenizer,
    train_dataset = dataset,
    max_seq_length = 2048,
    num_train_epochs = 3,
)

trainer.train()

# 5. Save LoRA adapter
model.save_pretrained("mealmind-nutrition-lora")
```

**Load fine-tuned model in C#:**
```csharp
// In Ollama, create a new model with fine-tuned weights
// Modelfile
"""
FROM qwen3:4b
ADAPTER ./mealmind-nutrition-lora
SYSTEM You are a nutrition expert assistant.
"""

// Use in backend
services.Configure<LlmOptions>(options =>
{
    options.ChatModel = "mealmind-nutrition-ft"; // Fine-tuned model
    options.EmbedModel = "qwen3-embedding:0.6b";
});
```

### Generating Training Data from RAG

```csharp
public class TrainingDataGenerator
{
    public async Task<List<TrainingExample>> GenerateFromRagLogs()
    {
        // Get successful RAG interactions
        var interactions = await _db.ChatLogs
            .Where(log => log.UserRating >= 4) // Only good responses
            .Take(1000)
            .ToListAsync();

        var trainingData = new List<TrainingExample>();

        foreach (var log in interactions)
        {
            // Extract: Question + Retrieved Docs + LLM Response
            var example = new TrainingExample
            {
                Instruction = log.UserQuery,
                Context = log.RetrievedDocuments, // Concatenated
                Output = log.LlmResponse
            };

            trainingData.Add(example);
        }

        // Export as JSONL for fine-tuning
        await File.WriteAllLinesAsync(
            "training_data.jsonl",
            trainingData.Select(JsonSerializer.Serialize)
        );

        return trainingData;
    }
}
```

### Continuous Fine-Tuning Pipeline

```csharp
public class ContinuousFineTuningService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Every week
            await Task.Delay(TimeSpan.FromDays(7), stoppingToken);

            // 1. Collect new high-quality interactions
            var newData = await CollectHighQualityInteractions();

            if (newData.Count < 100)
            {
                _logger.LogInformation("Not enough new data for fine-tuning");
                continue;
            }

            // 2. Trigger fine-tuning job
            await TriggerFineTuningJob(newData);

            // 3. Evaluate new model
            var metrics = await EvaluateNewModel();

            // 4. Deploy if better
            if (metrics.Accuracy > _currentModelMetrics.Accuracy)
            {
                await DeployNewModel();
                _logger.LogInformation("Deployed new fine-tuned model");
            }
        }
    }
}
```

---

## 5. Long-Context Processing

### Handling Documents > Context Window

**Problem:** Document is 50,000 tokens, but model context is 8,000 tokens.

**Solutions:**

### 1. Map-Reduce Pattern

```csharp
public class LongDocumentSummarizer
{
    public async Task<string> SummarizeLongDocument(string documentText)
    {
        // 1. MAP: Split into chunks that fit context window
        var chunks = SplitIntoChunks(documentText, maxTokens: 2000);

        // 2. MAP: Summarize each chunk
        var chunkSummaries = new List<string>();
        foreach (var chunk in chunks)
        {
            var summary = await _llm.GetResponseAsync($"""
                Summarize this section in 2-3 sentences:
                {chunk}
                """);
            chunkSummaries.Add(summary);
        }

        // 3. REDUCE: Combine summaries
        var combinedSummaries = string.Join("\n\n", chunkSummaries);

        // 4. REDUCE: Final summary
        var finalSummary = await _llm.GetResponseAsync($"""
            Create a comprehensive summary from these section summaries:
            {combinedSummaries}
            """);

        return finalSummary;
    }
}
```

### 2. Refine Pattern

```csharp
public async Task<string> RefineAnswer(string question, List<string> documentChunks)
{
    string answer = "No information yet.";

    foreach (var chunk in documentChunks)
    {
        // Each iteration refines the answer with new information
        var prompt = $"""
            Question: {question}

            Current Answer:
            {answer}

            New Information:
            {chunk}

            Refine the current answer using the new information.
            If the new information is not relevant, keep the current answer.
            """;

        answer = await _llm.GetResponseAsync(prompt);
    }

    return answer;
}
```

### 3. Hierarchical Summarization

```csharp
public class HierarchicalSummarizer
{
    public async Task<string> SummarizeBook(string bookText)
    {
        // Level 1: Paragraph summaries
        var paragraphs = SplitIntoParagraphs(bookText);
        var paragraphSummaries = await SummarizeBatch(paragraphs, "one sentence");

        // Level 2: Chapter summaries (group paragraphs)
        var chapters = GroupIntoChapters(paragraphSummaries);
        var chapterSummaries = await SummarizeBatch(chapters, "one paragraph");

        // Level 3: Book summary
        var bookSummary = await _llm.GetResponseAsync($"""
            Summarize this book based on chapter summaries:
            {string.Join("\n\n", chapterSummaries)}

            Write a 3-paragraph executive summary.
            """);

        return bookSummary;
    }

    private async Task<List<string>> SummarizeBatch(List<string> texts, string length)
    {
        var tasks = texts.Select(text =>
            _llm.GetResponseAsync($"Summarize in {length}: {text}")
        );

        var summaries = await Task.WhenAll(tasks);
        return summaries.ToList();
    }
}
```

### 4. Sliding Window Q&A

```csharp
public async Task<string> AnswerFromLongDocument(string question, string longDocument)
{
    var windowSize = 2000; // tokens
    var stride = 1000;     // overlap between windows

    var chunks = SlidingWindow(longDocument, windowSize, stride);
    var answers = new List<string>();

    foreach (var chunk in chunks)
    {
        var answer = await _llm.GetResponseAsync($"""
            Question: {question}
            Context: {chunk}

            Answer the question if information is present, otherwise say "Not found".
            """);

        if (!answer.Contains("Not found"))
        {
            answers.Add(answer);
        }
    }

    // Combine all relevant answers
    if (answers.Count == 0)
        return "Could not find answer in document.";

    if (answers.Count == 1)
        return answers[0];

    // Multiple answers found, consolidate
    return await _llm.GetResponseAsync($"""
        Combine these answers into one coherent response:
        {string.Join("\n---\n", answers)}
        """);
}
```

---

## 6. Semantic Caching

### Why Semantic Caching?

**Traditional caching:**
```csharp
// Exact match only
cache["How much protein?"] → cached result
cache["How much protein do I need?"] → MISS (different string)
```

**Semantic caching:**
```csharp
// Similarity match
cache[Embedding("How much protein?")] → cached result
cache[Embedding("How much protein do I need?")] → HIT! (98% similar)
```

### Implementation

```csharp
public class SemanticCache
{
    private readonly IMemoryCache _cache;
    private readonly IEmbeddingService _embeddingService;
    private const double SimilarityThreshold = 0.95;

    public async Task<string?> GetAsync(string prompt)
    {
        var promptEmbedding = await _embeddingService.GenerateAsync(prompt);

        // Check all cached embeddings
        var cacheKeys = GetAllCacheKeys();

        foreach (var key in cacheKeys)
        {
            var cached = _cache.Get<CachedResponse>(key);
            if (cached == null) continue;

            var similarity = CosineSimilarity(promptEmbedding, cached.PromptEmbedding);

            if (similarity >= SimilarityThreshold)
            {
                _logger.LogInformation(
                    "Semantic cache HIT: '{Original}' ≈ '{New}' (similarity: {Similarity})",
                    cached.OriginalPrompt,
                    prompt,
                    similarity
                );

                return cached.Response;
            }
        }

        return null; // Cache miss
    }

    public async Task SetAsync(string prompt, string response, TimeSpan ttl)
    {
        var embedding = await _embeddingService.GenerateAsync(prompt);
        var cacheKey = Guid.NewGuid().ToString();

        var cached = new CachedResponse
        {
            OriginalPrompt = prompt,
            PromptEmbedding = embedding,
            Response = response,
            CachedAt = DateTime.UtcNow
        };

        _cache.Set(cacheKey, cached, ttl);
    }
}

public record CachedResponse
{
    public string OriginalPrompt { get; init; }
    public Vector PromptEmbedding { get; init; }
    public string Response { get; init; }
    public DateTime CachedAt { get; init; }
}
```

### Usage

```csharp
public class CachedLlmService
{
    private readonly SemanticCache _cache;
    private readonly IChatClient _llm;

    public async Task<string> GetResponse(string prompt)
    {
        // Check semantic cache
        var cached = await _cache.GetAsync(prompt);
        if (cached != null)
        {
            return cached;
        }

        // Cache miss - call LLM
        var response = await _llm.GetResponseAsync(prompt);

        // Cache for 1 hour
        await _cache.SetAsync(prompt, response, TimeSpan.FromHours(1));

        return response;
    }
}
```

**Performance Impact:**
```
Without semantic cache:
  "How much protein?" → LLM call (2s)
  "How much protein do I need?" → LLM call (2s)
  "Protein requirements?" → LLM call (2s)
  Total: 6 seconds

With semantic cache:
  "How much protein?" → LLM call (2s) + cache
  "How much protein do I need?" → Cache HIT (10ms)
  "Protein requirements?" → Cache HIT (10ms)
  Total: 2.02 seconds (66% faster!)
```

### Advanced: Redis-Based Semantic Cache

```csharp
public class RedisSemanticCache
{
    private readonly IDatabase _redis;
    private readonly IEmbeddingService _embeddingService;

    public async Task<string?> GetAsync(string prompt)
    {
        var promptEmbedding = await _embeddingService.GenerateAsync(prompt);

        // Redis with RediSearch module supports vector similarity
        var query = new Query("*")
            .ReturnFields("prompt", "response")
            .SetSortBy("embedding", SortOrder.Asc)
            .Limit(1);

        var results = await _redis.FT().SearchAsync("cache_idx", query, new QueryOptions
        {
            VectorField = "embedding",
            VectorQuery = promptEmbedding.ToArray(),
            K = 1,
            DistanceMetric = "COSINE"
        });

        if (results.Documents.Count > 0 && results.Documents[0].Score >= SimilarityThreshold)
        {
            return results.Documents[0]["response"].ToString();
        }

        return null;
    }
}
```

---

## 7. LLM-Powered Backend Features

### 1. Natural Language to SQL

```csharp
public class NaturalLanguageQueryService
{
    public async Task<string> QueryDatabase(string naturalLanguageQuery)
    {
        // 1. Convert NL to SQL using LLM
        var sqlPrompt = $"""
            Convert this natural language query to SQL:
            "{naturalLanguageQuery}"

            Database schema:
            - Users (id, name, email, created_at)
            - FoodLogs (id, user_id, food_name, calories, logged_at)
            - UserProfiles (user_id, weight, height, goal)

            Return only the SQL query, no explanation.
            """;

        var sqlQuery = await _llm.GetResponseAsync(sqlPrompt);

        // 2. Validate SQL (security!)
        if (!IsValidSqlQuery(sqlQuery))
        {
            throw new SecurityException("Generated SQL is not safe");
        }

        // 3. Execute SQL
        var results = await _db.ExecuteRawSqlAsync(sqlQuery);

        // 4. Convert results to natural language answer
        var answerPrompt = $"""
            User asked: "{naturalLanguageQuery}"
            SQL results: {JsonSerializer.Serialize(results)}

            Provide a natural language answer to the user's question.
            """;

        return await _llm.GetResponseAsync(answerPrompt);
    }

    private bool IsValidSqlQuery(string sql)
    {
        // Only allow SELECT queries
        if (!sql.Trim().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            return false;

        // Block dangerous keywords
        var blocklist = new[] { "DROP", "DELETE", "UPDATE", "INSERT", "EXEC", "EXECUTE" };
        if (blocklist.Any(keyword => sql.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
            return false;

        return true;
    }
}

// User: "How many users logged food yesterday?"
// → SQL: SELECT COUNT(*) FROM Users u JOIN FoodLogs f ON u.id = f.user_id WHERE f.logged_at >= CURRENT_DATE - 1
// → Result: 47
// → Response: "47 users logged food yesterday."
```

### 2. Intelligent Form Validation

```csharp
public class IntelligentFormValidator
{
    public async Task<ValidationResult> ValidateUserInput(string fieldName, string value)
    {
        var prompt = $"""
            Validate this user input:
            Field: {fieldName}
            Value: "{value}"

            Check for:
            - Appropriate format
            - Reasonable values
            - Potential errors or typos

            Return JSON:
            {{
                "isValid": true/false,
                "suggestion": "corrected value or null",
                "message": "explanation"
            }}
            """;

        var result = await _llm.GetResponseAsync(prompt);
        return JsonSerializer.Deserialize<ValidationResult>(result);
    }
}

// User enters weight: "7 5kg"
// LLM suggests: "75kg" (noticed the space typo)

// User enters height: "5'11"
// LLM suggests: "180cm" (converted from imperial to metric)
```

### 3. Smart Notifications

```csharp
public class SmartNotificationService
{
    public async Task<string> GeneratePersonalizedNotification(Guid userId)
    {
        var userData = await GetUserContext(userId);

        var prompt = $"""
            Generate a motivational notification for this user:

            - Last food log: {userData.LastFoodLog} (3 days ago)
            - Goal: {userData.Goal}
            - Progress: {userData.ProgressPercent}%
            - Streak: {userData.Streak} days

            Create a personalized, encouraging message (max 100 chars).
            """;

        return await _llm.GetResponseAsync(prompt);
    }
}

// Output examples:
// "You're 73% to your goal! Log today to keep your 12-day streak alive 🔥"
// "Haven't seen you in 3 days. Small steps count - log one meal today!"
```

### 4. Automated Email Responses

```csharp
public class EmailResponseService
{
    public async Task<string> DraftResponseToUserEmail(string userEmail)
    {
        // 1. Classify email intent
        var intent = await ClassifyEmailIntent(userEmail);

        // 2. Generate appropriate response
        var prompt = $"""
            Draft a professional email response:

            User email:
            {userEmail}

            Intent: {intent}

            Guidelines:
            - Professional but friendly tone
            - Address their concern directly
            - Provide actionable next steps
            - Keep under 150 words
            """;

        return await _llm.GetResponseAsync(prompt);
    }

    private async Task<string> ClassifyEmailIntent(string email)
    {
        var prompt = $"""
            Classify this email intent:
            {email}

            Categories: bug_report, feature_request, account_issue, general_question, feedback

            Return only the category.
            """;

        return await _llm.GetResponseAsync(prompt);
    }
}
```

### 5. Content Moderation

```csharp
public class ContentModerationService
{
    public async Task<ModerationResult> ModerateUserContent(string content)
    {
        var prompt = $"""
            Analyze this user-generated content for policy violations:
            "{content}"

            Check for:
            - Hate speech
            - Harassment
            - Spam
            - Inappropriate health advice
            - Personal information

            Return JSON:
            {{
                "isAppropriate": true/false,
                "violations": ["violation1", ...],
                "severity": "low|medium|high",
                "suggestedAction": "allow|flag|block"
            }}
            """;

        var result = await _llm.GetResponseAsync(prompt);
        return JsonSerializer.Deserialize<ModerationResult>(result);
    }
}
```

---

## 8. Guardrails & Content Moderation

### Input Guardrails

```csharp
public class InputGuardrails
{
    public async Task<GuardrailResult> CheckInput(string userInput)
    {
        var checks = await Task.WhenAll(
            CheckForJailbreak(userInput),
            CheckForPII(userInput),
            CheckForInjection(userInput),
            CheckTopicRelevance(userInput)
        );

        var failed = checks.Where(c => !c.Passed).ToList();

        if (failed.Any())
        {
            return new GuardrailResult
            {
                Allowed = false,
                FailedChecks = failed.Select(c => c.CheckName).ToList(),
                Message = "Input violated content policy"
            };
        }

        return new GuardrailResult { Allowed = true };
    }

    private async Task<CheckResult> CheckForJailbreak(string input)
    {
        var suspiciousPatterns = new[]
        {
            "ignore previous",
            "disregard instructions",
            "pretend you are",
            "roleplay as",
            "new system prompt"
        };

        var hasJailbreak = suspiciousPatterns.Any(p =>
            input.Contains(p, StringComparison.OrdinalIgnoreCase));

        return new CheckResult
        {
            CheckName = "Jailbreak Detection",
            Passed = !hasJailbreak
        };
    }

    private async Task<CheckResult> CheckTopicRelevance(string input)
    {
        // Use LLM to check if input is relevant to nutrition/fitness
        var prompt = $"""
            Is this question relevant to nutrition, fitness, or health?
            Question: "{input}"

            Return only: YES or NO
            """;

        var response = await _llm.GetResponseAsync(prompt);
        var isRelevant = response.Trim().Equals("YES", StringComparison.OrdinalIgnoreCase);

        return new CheckResult
        {
            CheckName = "Topic Relevance",
            Passed = isRelevant
        };
    }
}
```

### Output Guardrails

```csharp
public class OutputGuardrails
{
    public async Task<string> FilterOutput(string llmOutput)
    {
        // 1. Check for harmful content
        if (await ContainsHarmfulAdvice(llmOutput))
        {
            return "I can't provide that information. Please consult a healthcare professional.";
        }

        // 2. Remove PII if leaked
        llmOutput = RedactPII(llmOutput);

        // 3. Check factual accuracy
        if (await ContainsFactualErrors(llmOutput))
        {
            llmOutput = await CorrectFactualErrors(llmOutput);
        }

        return llmOutput;
    }

    private async Task<bool> ContainsHarmfulAdvice(string output)
    {
        var prompt = $"""
            Does this nutrition advice contain potentially harmful recommendations?
            "{output}"

            Harmful = extremely low calories, dangerous supplements, medical claims

            Return: YES or NO
            """;

        var response = await _llm.GetResponseAsync(prompt);
        return response.Trim().Equals("YES", StringComparison.OrdinalIgnoreCase);
    }
}
```

### Constitutional AI Pattern

```csharp
public class ConstitutionalAI
{
    private readonly string[] _constitutionalPrinciples = new[]
    {
        "Do not provide medical diagnoses",
        "Do not recommend extreme diets under 1200 calories",
        "Always suggest consulting healthcare professionals for medical conditions",
        "Do not make guarantees about health outcomes",
        "Cite sources when making factual claims"
    };

    public async Task<string> GetConstitutionalResponse(string userQuery)
    {
        // 1. Initial response
        var initialResponse = await _llm.GetResponseAsync(userQuery);

        // 2. Self-critique against principles
        var critiquePrompt = $"""
            Review this response against these principles:
            {string.Join("\n", _constitutionalPrinciples.Select((p, i) => $"{i + 1}. {p}"))}

            Response to review:
            "{initialResponse}"

            Does it violate any principles? Return JSON:
            {{
                "violates": true/false,
                "violations": ["principle 1", ...],
                "suggestedRevision": "improved response"
            }}
            """;

        var critique = await _llm.GetResponseAsync(critiquePrompt);
        var critiqueResult = JsonSerializer.Deserialize<CritiqueResult>(critique);

        // 3. Return revised version if needed
        return critiqueResult.Violates
            ? critiqueResult.SuggestedRevision
            : initialResponse;
    }
}
```

---

## 9. Multi-Modal LLM Integration

### Vision Models in Backend

```csharp
public class FoodRecognitionService
{
    private readonly IVisionLlmClient _visionLlm;

    public async Task<FoodLogEntry> AnalyzeFoodImage(Stream imageStream)
    {
        // 1. Send image to vision model
        var prompt = """
            Analyze this food image and identify:
            1. Food items present
            2. Estimated portion sizes
            3. Approximate calories

            Return as JSON:
            {
                "foods": [
                    { "name": "food name", "portion": "amount", "calories": 123 }
                ]
            }
            """;

        var response = await _visionLlm.AnalyzeImageAsync(imageStream, prompt);
        var analysis = JsonSerializer.Deserialize<FoodAnalysis>(response);

        // 2. Create food log entry
        var entry = new FoodLogEntry
        {
            ImageUrl = await StoreImage(imageStream),
            DetectedFoods = analysis.Foods,
            TotalCalories = analysis.Foods.Sum(f => f.Calories),
            LoggedAt = DateTime.UtcNow
        };

        return entry;
    }
}

// User uploads photo of meal
// → Vision LLM detects: "Grilled chicken breast (200g, 330 cal), Brown rice (150g, 165 cal), Broccoli (100g, 34 cal)"
// → Auto-creates food log entry
```

### Text-to-Speech for Accessibility

```csharp
public class VoiceResponseService
{
    public async Task<Stream> GetVoiceResponse(string userQuery)
    {
        // 1. Generate text response
        var textResponse = await _llm.GetResponseAsync(userQuery);

        // 2. Convert to speech
        var audioStream = await _ttsService.SynthesizeAsync(textResponse, new SpeechOptions
        {
            Voice = "en-US-Neural",
            Speed = 1.0,
            Pitch = 0
        });

        return audioStream;
    }
}
```

### Speech-to-Text for Voice Commands

```csharp
public class VoiceCommandService
{
    public async Task<string> ProcessVoiceCommand(Stream audioStream)
    {
        // 1. Transcribe audio
        var transcription = await _sttService.TranscribeAsync(audioStream);

        // 2. Process with LLM
        var response = await _llm.GetResponseAsync(transcription);

        return response;
    }
}

// User says: "Log two eggs and toast for breakfast"
// → STT: "Log two eggs and toast for breakfast"
// → LLM: Understands intent, creates food log
// → Response: "Logged: 2 eggs (140 cal) and toast (80 cal)"
```

---

## 10. Batch Processing & Background Jobs

### Batch LLM Processing

```csharp
public class BatchLlmProcessor : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Check for pending batch jobs
            var pendingJobs = await _db.BatchJobs
                .Where(j => j.Status == BatchStatus.Pending)
                .Take(10)
                .ToListAsync(stoppingToken);

            foreach (var job in pendingJobs)
            {
                await ProcessBatchJob(job, stoppingToken);
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private async Task ProcessBatchJob(BatchJob job, CancellationToken ct)
    {
        job.Status = BatchStatus.Processing;
        job.StartedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        try
        {
            var results = new List<string>();

            foreach (var item in job.Items)
            {
                var response = await _llm.GetResponseAsync(item.Prompt, ct);
                results.Add(response);

                // Rate limiting
                await Task.Delay(TimeSpan.FromSeconds(1), ct);
            }

            job.Results = JsonSerializer.Serialize(results);
            job.Status = BatchStatus.Completed;
            job.CompletedAt = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            job.Status = BatchStatus.Failed;
            job.ErrorMessage = ex.Message;
        }

        await _db.SaveChangesAsync(ct);
    }
}
```

### Use Case: Weekly Nutrition Reports

```csharp
public class WeeklyReportGenerator : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Run every Sunday at 8 AM
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var nextSunday = GetNextSunday(now);
            var delay = nextSunday - now;

            await Task.Delay(delay, stoppingToken);

            await GenerateWeeklyReports(stoppingToken);
        }
    }

    private async Task GenerateWeeklyReports(CancellationToken ct)
    {
        var users = await _db.Users.Where(u => u.EmailNotifications).ToListAsync(ct);

        foreach (var user in users)
        {
            try
            {
                var report = await GenerateUserReport(user.Id, ct);
                await _emailService.SendAsync(user.Email, "Your Weekly Nutrition Report", report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate report for user {UserId}", user.Id);
            }

            // Rate limiting
            await Task.Delay(TimeSpan.FromSeconds(2), ct);
        }
    }

    private async Task<string> GenerateUserReport(Guid userId, CancellationToken ct)
    {
        var weekData = await GetUserWeekData(userId);

        var prompt = $"""
            Generate a personalized weekly nutrition report:

            User Data:
            - Total calories consumed: {weekData.TotalCalories}
            - Average daily: {weekData.AverageDailyCalories}
            - Target: {weekData.TargetCalories}
            - Protein: {weekData.TotalProtein}g (avg {weekData.AverageProtein}g/day)
            - Days logged: {weekData.DaysLogged}/7

            Create a report with:
            1. Progress summary (2-3 sentences)
            2. Achievements and highlights
            3. Areas for improvement
            4. One actionable tip for next week

            Tone: Encouraging, specific, data-driven
            """;

        return await _llm.GetResponseAsync(prompt, ct);
    }
}
```

---

## 11. LLM API Gateway Pattern

### Centralized LLM Gateway

```csharp
public class LlmGateway
{
    private readonly Dictionary<string, ILlmProvider> _providers;
    private readonly ILlmRoutingStrategy _routingStrategy;

    public LlmGateway(
        IEnumerable<ILlmProvider> providers,
        ILlmRoutingStrategy routingStrategy)
    {
        _providers = providers.ToDictionary(p => p.Name);
        _routingStrategy = routingStrategy;
    }

    public async Task<string> GetResponseAsync(LlmRequest request)
    {
        // 1. Select provider based on routing strategy
        var provider = await _routingStrategy.SelectProviderAsync(request, _providers.Values);

        // 2. Apply middleware (logging, rate limiting, etc.)
        await ApplyMiddleware(request);

        // 3. Call provider with retry logic
        var response = await ExecuteWithRetry(provider, request);

        // 4. Track metrics
        await TrackMetrics(provider.Name, request, response);

        return response.Text;
    }

    private async Task<LlmResponse> ExecuteWithRetry(ILlmProvider provider, LlmRequest request)
    {
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))
            );

        return await retryPolicy.ExecuteAsync(async () =>
            await provider.GetResponseAsync(request)
        );
    }
}
```

### Smart Routing Strategies

```csharp
public class CostOptimizedRoutingStrategy : ILlmRoutingStrategy
{
    public async Task<ILlmProvider> SelectProviderAsync(
        LlmRequest request,
        IEnumerable<ILlmProvider> providers)
    {
        // Simple queries → cheap provider
        if (request.EstimatedTokens < 500)
        {
            return providers.First(p => p.Name == "ollama-local");
        }

        // Complex queries → quality provider
        if (request.RequiresReasoning)
        {
            return providers.First(p => p.Name == "gpt-4");
        }

        // Default → balanced provider
        return providers.First(p => p.Name == "claude-haiku");
    }
}

public class LoadBalancingRoutingStrategy : ILlmRoutingStrategy
{
    public async Task<ILlmProvider> SelectProviderAsync(
        LlmRequest request,
        IEnumerable<ILlmProvider> providers)
    {
        // Round-robin across healthy providers
        var healthyProviders = providers.Where(p => p.IsHealthy).ToList();

        if (!healthyProviders.Any())
        {
            throw new Exception("No healthy LLM providers available");
        }

        var index = Interlocked.Increment(ref _counter) % healthyProviders.Count;
        return healthyProviders[index];
    }

    private static int _counter = 0;
}
```

### Fallback Chain

```csharp
public class FallbackLlmClient : ILlmClient
{
    private readonly List<ILlmProvider> _providerChain;

    public FallbackLlmClient(List<ILlmProvider> providerChain)
    {
        _providerChain = providerChain; // Ordered by preference
    }

    public async Task<string> GetResponseAsync(string prompt)
    {
        Exception lastException = null;

        foreach (var provider in _providerChain)
        {
            try
            {
                _logger.LogInformation("Attempting provider: {Provider}", provider.Name);
                var response = await provider.GetResponseAsync(prompt);
                return response.Text;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Provider {Provider} failed, trying next", provider.Name);
                lastException = ex;
            }
        }

        throw new Exception("All LLM providers failed", lastException);
    }
}

// Configuration
services.AddSingleton<ILlmClient>(sp => new FallbackLlmClient(new List<ILlmProvider>
{
    sp.GetRequiredService<OllamaProvider>(),     // Try local first (fast, free)
    sp.GetRequiredService<ClaudeHaikuProvider>(), // Then cheap cloud
    sp.GetRequiredService<GPT4Provider>()         // Finally, premium
}));
```

---

## 12. State Management for Conversations

### Conversation State Machine

```csharp
public class ConversationStateMachine
{
    public enum ConversationState
    {
        Initial,
        CollectingUserInfo,
        CalculatingCalories,
        CreatingMealPlan,
        ReviewingPlan,
        Complete
    }

    public async Task<StateTransition> ProcessMessage(Guid conversationId, string userMessage)
    {
        var conversation = await _db.Conversations.FindAsync(conversationId);
        var currentState = conversation.State;

        var transition = currentState switch
        {
            ConversationState.Initial => await HandleInitial(userMessage),
            ConversationState.CollectingUserInfo => await HandleCollectingInfo(userMessage, conversation),
            ConversationState.CalculatingCalories => await HandleCalculating(conversation),
            ConversationState.CreatingMealPlan => await HandleCreatingPlan(conversation),
            ConversationState.ReviewingPlan => await HandleReviewing(userMessage, conversation),
            _ => throw new InvalidOperationException($"Unknown state: {currentState}")
        };

        // Update conversation state
        conversation.State = transition.NextState;
        conversation.StateData = transition.StateData;
        await _db.SaveChangesAsync();

        return transition;
    }

    private async Task<StateTransition> HandleCollectingInfo(string message, Conversation conversation)
    {
        // Use LLM to extract information
        var prompt = $"""
            Extract user information from this message:
            "{message}"

            Current data: {conversation.StateData}

            Return JSON with extracted fields:
            {{
                "weight": number or null,
                "height": number or null,
                "age": number or null,
                "goal": "muscle_gain|fat_loss|maintain" or null,
                "activityLevel": "sedentary|light|moderate|very_active" or null
            }}

            Also indicate if we have all required information:
            {{ "isComplete": true/false }}
            """;

        var response = await _llm.GetResponseAsync(prompt);
        var extracted = JsonSerializer.Deserialize<UserInfoExtraction>(response);

        if (extracted.IsComplete)
        {
            return new StateTransition
            {
                NextState = ConversationState.CalculatingCalories,
                StateData = JsonSerializer.Serialize(extracted),
                Message = "Great! Let me calculate your calorie needs..."
            };
        }

        // Ask for missing information
        var missingFields = GetMissingFields(extracted);
        return new StateTransition
        {
            NextState = ConversationState.CollectingUserInfo,
            StateData = JsonSerializer.Serialize(extracted),
            Message = $"Could you provide your {string.Join(" and ", missingFields)}?"
        };
    }
}
```

### Persistent Context Across Sessions

```csharp
public class ConversationContextManager
{
    public async Task<ConversationContext> BuildContext(Guid conversationId)
    {
        var conversation = await _db.Conversations
            .Include(c => c.Messages)
            .Include(c => c.User)
            .FirstAsync(c => c.Id == conversationId);

        var context = new ConversationContext
        {
            UserId = conversation.UserId,
            UserProfile = await GetUserProfile(conversation.UserId),
            ConversationHistory = GetRelevantHistory(conversation.Messages),
            ConversationGoal = conversation.Goal,
            CurrentTopic = await InferCurrentTopic(conversation.Messages),
            UserPreferences = await GetUserPreferences(conversation.UserId)
        };

        return context;
    }

    private async Task<string> InferCurrentTopic(List<Message> messages)
    {
        var recentMessages = messages.TakeLast(5);

        var prompt = $"""
            Based on these recent messages, what topic is the user discussing?
            {string.Join("\n", recentMessages.Select(m => $"{m.Role}: {m.Content}"))}

            Topics: nutrition, training, recovery, supplements, general
            Return only the topic.
            """;

        return await _llm.GetResponseAsync(prompt);
    }

    private string GetRelevantHistory(List<Message> allMessages)
    {
        // Sliding window + importance sampling
        var recentMessages = allMessages.TakeLast(10);
        var importantMessages = allMessages
            .Where(m => m.IsImportant) // User feedback, key decisions
            .TakeLast(5);

        var relevantMessages = recentMessages.Union(importantMessages).Distinct();

        return string.Join("\n", relevantMessages.Select(m => $"{m.Role}: {m.Content}"));
    }
}
```

---

## 13. Prompt Versioning & A/B Testing

### Prompt Version Management

```csharp
public class PromptVersionManager
{
    public async Task<string> GetPrompt(string promptKey, string version = "latest")
    {
        var promptVersion = version == "latest"
            ? await _db.PromptVersions
                .Where(p => p.Key == promptKey && p.IsActive)
                .OrderByDescending(p => p.Version)
                .FirstAsync()
            : await _db.PromptVersions
                .FirstAsync(p => p.Key == promptKey && p.Version == version);

        return promptVersion.Template;
    }

    public async Task CreateNewVersion(string promptKey, string template, string changelog)
    {
        var latestVersion = await _db.PromptVersions
            .Where(p => p.Key == promptKey)
            .OrderByDescending(p => p.Version)
            .Select(p => p.Version)
            .FirstOrDefaultAsync();

        var newVersion = new PromptVersion
        {
            Key = promptKey,
            Version = IncrementVersion(latestVersion),
            Template = template,
            Changelog = changelog,
            CreatedAt = DateTime.UtcNow,
            IsActive = false // Require manual activation
        };

        await _db.PromptVersions.AddAsync(newVersion);
        await _db.SaveChangesAsync();
    }
}

// Usage
var prompt = await _promptVersionManager.GetPrompt("nutrition-qa", version: "1.2.3");
var response = await _llm.GetResponseAsync(prompt);
```

### A/B Testing Framework

```csharp
public class PromptABTestService
{
    public async Task<string> GetPromptForUser(Guid userId, string promptKey)
    {
        var activeTest = await _db.ABTests
            .FirstOrDefaultAsync(t => t.PromptKey == promptKey && t.IsActive);

        if (activeTest == null)
        {
            // No A/B test, use default
            return await _promptVersionManager.GetPrompt(promptKey);
        }

        // Assign user to variant (consistent hashing)
        var variant = GetVariantForUser(userId, activeTest);

        // Track assignment
        await TrackAssignment(userId, activeTest.Id, variant);

        return variant == "A" ? activeTest.PromptA : activeTest.PromptB;
    }

    private string GetVariantForUser(Guid userId, ABTest test)
    {
        // Consistent hashing ensures same user always gets same variant
        var hash = userId.GetHashCode();
        return (hash % 2) == 0 ? "A" : "B";
    }

    public async Task TrackOutcome(Guid userId, string promptKey, OutcomeMetrics metrics)
    {
        var assignment = await _db.ABTestAssignments
            .Where(a => a.UserId == userId && a.ABTest.PromptKey == promptKey)
            .OrderByDescending(a => a.AssignedAt)
            .FirstAsync();

        await _db.ABTestResults.AddAsync(new ABTestResult
        {
            ABTestId = assignment.ABTestId,
            Variant = assignment.Variant,
            UserRating = metrics.UserRating,
            ResponseTime = metrics.ResponseTime,
            TokensUsed = metrics.TokensUsed,
            Timestamp = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
    }
}

// Analyze results
public async Task<ABTestAnalysis> AnalyzeABTest(Guid testId)
{
    var results = await _db.ABTestResults
        .Where(r => r.ABTestId == testId)
        .ToListAsync();

    var variantA = results.Where(r => r.Variant == "A");
    var variantB = results.Where(r => r.Variant == "B");

    return new ABTestAnalysis
    {
        VariantA = new VariantMetrics
        {
            SampleSize = variantA.Count(),
            AverageRating = variantA.Average(r => r.UserRating),
            AverageResponseTime = variantA.Average(r => r.ResponseTime),
            AverageTokensUsed = variantA.Average(r => r.TokensUsed)
        },
        VariantB = new VariantMetrics
        {
            SampleSize = variantB.Count(),
            AverageRating = variantB.Average(r => r.UserRating),
            AverageResponseTime = variantB.Average(r => r.ResponseTime),
            AverageTokensUsed = variantB.Average(r => r.TokensUsed)
        },
        StatisticalSignificance = CalculateSignificance(variantA, variantB)
    };
}
```

---

## 14. LLM-Driven Workflows

### Workflow Orchestration

```csharp
public class OnboardingWorkflow
{
    public async Task<WorkflowResult> RunOnboardingWorkflow(Guid userId)
    {
        var workflow = new WorkflowBuilder()
            .AddStep("collect_basic_info", async () =>
            {
                var info = await CollectUserInfo(userId);
                return new { info };
            })
            .AddStep("calculate_targets", async (context) =>
            {
                var userInfo = context["info"];
                var targets = await CalculateTargets(userInfo);
                return new { targets };
            })
            .AddStep("generate_meal_plan", async (context) =>
            {
                var targets = context["targets"];
                var mealPlan = await GenerateMealPlan(targets);
                return new { mealPlan };
            })
            .AddStep("send_welcome_email", async (context) =>
            {
                var mealPlan = context["mealPlan"];
                await SendWelcomeEmail(userId, mealPlan);
                return null;
            })
            .Build();

        return await workflow.ExecuteAsync();
    }

    private async Task<UserInfo> CollectUserInfo(Guid userId)
    {
        // Multi-turn conversation with LLM to collect info
        var conversation = await _conversationService.StartConversation(userId, "onboarding");

        while (!conversation.IsComplete)
        {
            var userMessage = await WaitForUserMessage(conversation.Id);
            var llmResponse = await _llm.GetResponseAsync(BuildOnboardingPrompt(conversation, userMessage));

            conversation = await ProcessOnboardingResponse(conversation, llmResponse);
        }

        return ExtractUserInfo(conversation);
    }
}
```

### Event-Driven LLM Workflows

```csharp
public class EventDrivenWorkflowHandler
{
    // When user logs food
    [EventHandler("FoodLogged")]
    public async Task HandleFoodLogged(FoodLoggedEvent @event)
    {
        // Check if user exceeded daily target
        var dailyTotal = await GetDailyTotal(@event.UserId);
        var target = await GetUserTarget(@event.UserId);

        if (dailyTotal.Calories > target.Calories * 1.2) // 20% over
        {
            // Generate personalized feedback
            var feedback = await _llm.GetResponseAsync($"""
                User exceeded calorie target:
                - Target: {target.Calories}
                - Actual: {dailyTotal.Calories}
                - Difference: {dailyTotal.Calories - target.Calories}

                Generate encouraging feedback (2 sentences):
                1. Acknowledge the overage without judgment
                2. Provide one actionable tip for tomorrow
                """);

            // Send notification
            await _notificationService.SendAsync(@event.UserId, feedback);
        }
    }

    // When user skips logging for 3 days
    [EventHandler("MissedLoggingStreak")]
    public async Task HandleMissedLogging(MissedLoggingEvent @event)
    {
        var userContext = await GetUserContext(@event.UserId);

        var message = await _llm.GetResponseAsync($"""
            User hasn't logged food in 3 days:
            - Previous streak: {@event.PreviousStreak} days
            - Goal: {userContext.Goal}
            - Past success rate: {userContext.SuccessRate}%

            Generate a re-engagement message (3 sentences):
            1. Acknowledge the break (non-judgmental)
            2. Remind of progress made
            3. Encourage easy first step
            """);

        await _emailService.SendAsync(@event.UserId, "We miss you!", message);
    }
}
```

---

## 15. Real-Time Streaming & WebSockets

### Server-Sent Events (SSE) Implementation

```csharp
[HttpGet("chat/stream")]
public async Task StreamChatResponse(string prompt)
{
    Response.Headers.Add("Content-Type", "text/event-stream");
    Response.Headers.Add("Cache-Control", "no-cache");
    Response.Headers.Add("Connection", "keep-alive");

    await foreach (var chunk in _llm.StreamResponseAsync(prompt))
    {
        var sseMessage = $"data: {JsonSerializer.Serialize(new { chunk })}\n\n";
        await Response.WriteAsync(sseMessage);
        await Response.Body.FlushAsync();
    }

    await Response.WriteAsync("data: [DONE]\n\n");
    await Response.Body.FlushAsync();
}
```

### WebSocket Chat Implementation

```csharp
public class ChatWebSocketHandler : WebSocketHandler
{
    public override async Task OnConnected(WebSocket socket)
    {
        await base.OnConnected(socket);

        var socketId = WebSocketManager.GetId(socket);
        _logger.LogInformation("Client connected: {SocketId}", socketId);
    }

    public override async Task OnMessage(WebSocket socket, string message)
    {
        var request = JsonSerializer.Deserialize<ChatRequest>(message);

        // Start streaming response
        await foreach (var chunk in _llm.StreamResponseAsync(request.Prompt))
        {
            var response = new
            {
                type = "chunk",
                content = chunk,
                conversationId = request.ConversationId
            };

            await SendMessageAsync(socket, JsonSerializer.Serialize(response));
        }

        // Send completion marker
        await SendMessageAsync(socket, JsonSerializer.Serialize(new { type = "done" }));
    }
}

// Startup configuration
app.UseWebSockets();
app.MapWebSocketHandler("/ws/chat", new ChatWebSocketHandler());
```

### Progressive Enhancement Pattern

```csharp
public class ProgressiveLlmResponse
{
    public async Task<StructuredResponse> GetProgressiveResponse(
        string prompt,
        Func<PartialResponse, Task> onProgress)
    {
        var partialResponse = new PartialResponse();

        await foreach (var chunk in _llm.StreamResponseAsync(prompt))
        {
            partialResponse.AddChunk(chunk);

            // Send intermediate results as they become available
            if (partialResponse.HasCompleteSection())
            {
                await onProgress(partialResponse);
            }
        }

        // Final parsing
        return JsonSerializer.Deserialize<StructuredResponse>(partialResponse.FullText);
    }
}

// Usage
await _progressiveLlm.GetProgressiveResponse(
    "Explain protein requirements",
    async (partial) =>
    {
        // Send partial results to client via WebSocket
        await _webSocket.SendAsync(new
        {
            type = "partial",
            title = partial.ExtractedTitle,
            firstParagraph = partial.ExtractedParagraphs.FirstOrDefault()
        });
    }
);
```

---

## Summary & Learning Path

### Essential Skills for LLM Backend Development

**Foundation (Weeks 1-4):**
- ✅ Basic LLM integration (API clients)
- ✅ Prompt engineering
- ✅ Structured output (JSON mode)
- ✅ Error handling and retries

**Intermediate (Months 2-3):**
- ⬜ Function calling & tool use
- ⬜ Embeddings and semantic search
- ⬜ RAG systems
- ⬜ Conversation state management
- ⬜ Streaming responses

**Advanced (Months 4-6):**
- ⬜ LLM agents & orchestration
- ⬜ Fine-tuning for domain
- ⬜ Multi-modal integration
- ⬜ A/B testing frameworks
- ⬜ Production monitoring

**Expert (Months 6+):**
- ⬜ Custom agent frameworks
- ⬜ LLM workflow orchestration
- ⬜ Cost optimization strategies
- ⬜ Security & guardrails
- ⬜ Performance optimization

### Key Takeaways

1. **LLMs are more than chatbots** - Use for classification, extraction, validation, generation
2. **Function calling is powerful** - Let LLMs orchestrate your backend logic
3. **Embeddings enable new patterns** - Semantic search, recommendations, clustering
4. **Agents automate complex workflows** - Multi-step reasoning and tool use
5. **Production requires guardrails** - Input/output validation, rate limiting, monitoring
6. **Streaming improves UX** - Don't make users wait for full responses
7. **Test and iterate** - A/B test prompts, track metrics, improve continuously

### Recommended Next Steps

**Build Projects:**
1. **Function-calling assistant** - Natural language to database queries
2. **Content moderation system** - Automated policy enforcement
3. **Intelligent search** - Semantic + keyword hybrid search
4. **Automation agent** - Multi-step workflow orchestration
5. **Recommendation engine** - Embedding-based personalization

**Study Resources:**
- **LangChain Documentation** - Patterns and examples
- **Semantic Kernel Docs** - .NET-specific LLM patterns
- **OpenAI Cookbook** - Best practices and recipes
- **Anthropic's Prompt Engineering Guide** - Advanced techniques
- **Papers with Code (NLP)** - Latest research

---

**Last Updated:** 2025-10-24
**Author:** Michał Drozd (MealMind Project)
