using System.Buffers.Text;
using System.Text.Json;
using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Modules.AiChat.Application.Dtos;
using MealMind.Shared.Abstractions.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ChatMessageContent = Microsoft.SemanticKernel.ChatMessageContent;
using ChatResponseFormat = Microsoft.Extensions.AI.ChatResponseFormat;

namespace MealMind.Modules.AiChat.Infrastructure.Services;

public class AiChatService : IAiChatService
{
    private readonly IChatCompletionService _chatCompletionService;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private static readonly string[] Value = ["title", "paragraphs", "keyPoints", "sources"];

    public AiChatService(IChatCompletionService chatCompletionService)
    {
        _chatCompletionService = chatCompletionService;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
    }

    //mayby make it T GenerateStructuredResponseAsync<T>
    public async Task<StructuredResponse> GenerateStructuredResponseAsync(string userPrompt, string documentsText,
        List<ChatMessageContent> chatMessages, int responseTokensLimit,
        CancellationToken cancellationToken = default)
    {
        var systemPrompt =
            $$"""
              You are a nutrition assistant. Answer using facts from the reference documents below.

              ═══════════════════════════════════════════════════════════════
              📚 REFERENCE DOCUMENTS
              ═══════════════════════════════════════════════════════════════
              {{documentsText}}

              ═══════════════════════════════════════════════════════════════
              📋 RESPONSE REQUIREMENTS
              ═══════════════════════════════════════════════════════════════
              Answer with factual details from documents above. Include:

              • Title: Specific and descriptive to the user's question
              • Paragraphs: 2-4 detailed paragraphs (100-250 words each)
                → Use concrete data, numbers, ranges, and mechanisms from documents
                → Include specific nutritional values, scientific findings, or practical recommendations
                → No generic summaries or placeholder text
              • KeyPoints: 3-7 concise facts (10-30 words each)
                → One-sentence summaries of the most important information
                → Focus on actionable takeaways or key numbers

              Example response:
              {
                "Title": "Protein Requirements for Fat Loss Phase",
                "Paragraphs": [
                  "During a cutting phase, protein intake should be 2.0–2.4 grams per kilogram of body weight to preserve lean muscle mass while in a calorie deficit.",
                  "This range is higher than the muscle gain recommendation (1.6–2.2 g/kg) because protein helps prevent muscle breakdown when calories are restricted."
                ],
                "KeyPoints": [
                  "Cutting phase: 2.0–2.4 g/kg body weight",
                  "Spread protein across 3–5 meals daily",
                  "Use complete protein sources like eggs, meat, fish, dairy"
                ]
              }

              ═══════════════════════════════════════════════════════════════
              ⚠️ BEFORE RESPONDING - VERIFY
              ═══════════════════════════════════════════════════════════════
              1. Did I include specific factual data from documents (not generic statements)?
              2. Are my paragraphs detailed with concrete numbers and explanations?
              3. Are my key points concise and actionable?
              4. Is my JSON valid (no markdown fences, proper formatting)?

              Output pure JSON only (first character '{', last character '}'):
              """;

        var systemMessage = new ChatMessageContent(AuthorRole.System, systemPrompt);
        chatMessages.Add(systemMessage);

        var userMessage = new ChatMessageContent(AuthorRole.User, userPrompt);
        chatMessages.Add(userMessage);

        var chatHistory = new ChatHistory(chatMessages);

        var response = await _chatCompletionService.GetChatMessageContentsAsync(chatHistory, new OpenAIPromptExecutionSettings
        {
            ChatSystemPrompt = systemPrompt,
            MaxTokens = responseTokensLimit,
            Temperature = 0.5f,

            ResponseFormat = typeof(StructuredResponse)
            // WebSearchOptions = null
            // ReasoningEffort = ChatReasoningEffortLevel.Medium
            // ReasoningEffort = "medium"
        }, cancellationToken: cancellationToken);

        var responseText = response[0].Content;

        if (string.IsNullOrWhiteSpace(responseText))
            throw new ArgumentNullException(responseText);

        if (!responseText.StartsWith('{') ||
            !responseText.EndsWith('}'))
        {
            var repairedJson =
                await AttemptJsonCorrectionAsync(userPrompt, responseText, documentsText, responseTokensLimit, cancellationToken);

            return repairedJson;
        }

        var structuredResponse = JsonSerializer.Deserialize<StructuredResponse>(responseText, _jsonSerializerOptions)!;

        return structuredResponse;
    }

    public async Task<AnalyzedImageStructuredResponse> GenerateTextToImagePromptAsync(string? userPrompt, IFormFile imageFile,
        CancellationToken cancellationToken = default)
    {
        var imageBytes = await imageFile.ToReadOnlyMemoryByteArrayAsync(cancellationToken);
        var systemPrompt =
            $$"""
              You are an AI food nutrition analyst that examines food images and returns detailed nutritional estimates in structured JSON format.

              ═══════════════════════════════════════════════════════════════
              📸 IMAGE ANALYSIS INSTRUCTIONS
              ═══════════════════════════════════════════════════════════════
              Analyze the provided food image and identify:

              1. **Individual Food Items**: Detect each distinct food item separately
                 - Examples: "Grilled chicken breast", "Steamed broccoli", "Brown rice"
                 - For composite meals (sandwiches, burgers): break down into components if nutritionally significant

              2. **Quantity Estimation**: Estimate weight/volume in grams or milliliters
                 - Use visual cues: plate size, food dimensions, thickness
                 - Compare to common reference sizes (palm, fist, deck of cards)
                 - Provide ranges when uncertain (e.g., 150-200g)

              3. **Cooking Methods**: Identify preparation style as it affects calories
                 - Examples: grilled, fried, baked, raw, steamed, breaded
                 - Note added fats: "fried in oil", "buttered", "with sauce"

              4. **User Input Priority & Mixed Analysis**:
                 - ALWAYS detect ALL food items visible in the image
                 - If user provides specific details for certain foods, use those exact values with confidence = 1.0
                 - For foods NOT mentioned by user, estimate based on visual analysis with appropriate confidence (0.5-0.95)

                 Examples:
                 - User says "200g chicken" but image shows chicken + rice + broccoli
                   → Chicken: 200g (confidence 1.0), Rice: estimate visually (confidence 0.8), Broccoli: estimate visually (confidence 0.9)
                 - User says "grilled chicken" but you see fried chicken in image
                   → Trust user: mark as grilled (confidence 1.0)

              ═══════════════════════════════════════════════════════════════
              📊 NUTRITION ESTIMATION GUIDELINES
              ═══════════════════════════════════════════════════════════════
              For EACH detected food item, estimate:

              • **Calories**: Provide min-max range based on preparation method
                - Grilled chicken (150g): 240-280 kcal
                - Fried chicken (150g): 350-450 kcal

              • **Macronutrients**: Estimate protein, fats, carbohydrates (min-max ranges)
                - Protein-rich foods: meat, fish, eggs, dairy, legumes
                - Fat content varies by cooking method (grilled vs fried)
                - Carbs: grains, bread, pasta, fruits, starchy vegetables

              • **Confidence Score**: Rate your estimation accuracy (0.0 to 1.0)
                - 1.0: Absolute certainty (always when user provides exact details)
                - 0.9-1.0: Clear view, recognizable food, standard portion
                - 0.7-0.89: Partially obscured or unusual preparation
                - 0.5-0.69: Uncertain identification or very non-standard portion
                - Below 0.5: Cannot reliably identify

              • **Micronutrients** (OPTIONAL - only if user mentions them):
                - Sugars, saturated fats, fiber, sodium, salt, cholesterol
                - Leave as null if not explicitly requested or obvious (e.g., soda = high sugar)

              ═══════════════════════════════════════════════════════════════
              🎯 USER PROMPT CONTEXT
              ═══════════════════════════════════════════════════════════════
              User input: "{{userPrompt ?? "[No text provided - analyze image only]"}}"

              CRITICAL INSTRUCTION - MIXED ANALYSIS APPROACH:
              1. ALWAYS detect and analyze ALL food items visible in the image
              2. For foods mentioned in user prompt with specific details:
                 → Use user's exact values (weights, cooking methods)
                 → Set confidence = 1.0 for those items
              3. For foods NOT mentioned in user prompt:
                 → Estimate based on visual analysis
                 → Set confidence based on visual clarity (0.5-0.95)
              4. Never ignore foods in the image just because user didn't mention them

              If user prompt is empty or vague:
              → Analyze all visible foods
              → Use visual estimation for all values
              → Provide ranges to indicate uncertainty

              ═══════════════════════════════════════════════════════════════
              📋 REQUIRED JSON RESPONSE FORMAT
              ═══════════════════════════════════════════════════════════════
              Return a JSON object matching this exact structure:
              If {{userPrompt}} is null or empty, adjust UserDescription accordingly.

              {
                "DetectedFoods": [
                  {
                    "FoodName": "Grilled Chicken Breast",
                    "QuantityInGrams": 180.0,
                    "ConfidenceScore": 0.85,
                    "MinEstimatedCalories": 280.0,
                    "MaxEstimatedCalories": 320.0,
                    "MinEstimatedProteins": 52.0,
                    "MaxEstimatedProteins": 58.0,
                    "MinEstimatedFats": 6.0,
                    "MaxEstimatedFats": 9.0,
                    "MinEstimatedCarbohydrates": 0.0,
                    "MaxEstimatedCarbohydrates": 0.5
                  },
                  {
                    "FoodName": "Steamed Broccoli",
                    "QuantityInGrams": 100.0,
                    "ConfidenceScore": 0.90,
                    "MinEstimatedCalories": 30.0,
                    "MaxEstimatedCalories": 40.0,
                    "MinEstimatedProteins": 2.5,
                    "MaxEstimatedProteins": 3.5,
                    "MinEstimatedFats": 0.3,
                    "MaxEstimatedFats": 0.5,
                    "MinEstimatedCarbohydrates": 5.0,
                    "MaxEstimatedCarbohydrates": 7.0
                  }
                ],
                "UserDescription": {{userPrompt}}
              }

              ═══════════════════════════════════════════════════════════════
              ⚠️ CRITICAL VALIDATION RULES
              ═══════════════════════════════════════════════════════════════
              Before submitting your response, verify:

              ✓ Each food item has Min ≤ Max for all nutritional values
              ✓ DO NOT calculate total nutrition fields - they will be computed automatically from individual foods
              ✓ Confidence scores are between 0.0 and 1.0
              ✓ All required fields are present (no null values for required fields)
              ✓ QuantityInGrams is positive number
              ✓ JSON is valid (proper escaping, no trailing commas, no comments)
              ✓ If there is user prompt remeb
              ✓ UserDescription accurately reflects user input. If no input, just put null"


              Output pure JSON only (first character '{', last character '}'):
              """;

        var systemMessage = new ChatMessageContent(AuthorRole.System, systemPrompt);

        var chatHistory = new ChatHistory();

        var complexUserMessage = new ChatMessageContent(AuthorRole.User, new ChatMessageContentItemCollection
        {
            new TextContent(userPrompt ?? string.Empty),
            new ImageContent(imageBytes, "image/jpeg")
        });

        chatHistory.AddRange([systemMessage, complexUserMessage]);

        var response = await _chatCompletionService.GetChatMessageContentsAsync(chatHistory, new OpenAIPromptExecutionSettings
        {
            ChatSystemPrompt = systemPrompt,
            MaxTokens = 1000, // Increased for multi-food analysis
            Temperature = 0.3f, // Lower for more factual responses
            ResponseFormat = typeof(AnalyzedImageStructuredResponse)
        }, cancellationToken: cancellationToken);

        var responseText = response[0].Content;

        if (string.IsNullOrWhiteSpace(responseText))
            throw new InvalidOperationException("Vision model returned empty response");

        var structuredResponse = JsonSerializer.Deserialize<AnalyzedImageStructuredResponse>(responseText, _jsonSerializerOptions)!;

        return structuredResponse;
    }

    private async Task<StructuredResponse> AttemptJsonCorrectionAsync(string originalQuestion, string malformedJson, string documentsText,
        int responseTokensLimit,
        CancellationToken cancellationToken = default)
    {
        var systemPrompt =
            $$"""
              You are a JSON correction assistant. Fix the malformed JSON response below.

              ═══════════════════════════════════════════════════════════════
              📚 REFERENCE DOCUMENTS
              ═══════════════════════════════════════════════════════════════
              {{documentsText}}

              ═══════════════════════════════════════════════════════════════
              🧩 TASK
              ═══════════════════════════════════════════════════════════════
              User question: "{{originalQuestion}}"

              Fix the malformed JSON below to match this schema:
              {
                "Title": "string",
                "Paragraphs": ["string", "string", ...],
                "KeyPoints": ["string", "string", ...]
              }

              RULES:
              1. Output ONLY valid JSON - NO markdown, NO explanations
              2. Keep the content meaning intact
              3. Ensure proper escaping of quotes and special characters
              4. Title: Specific to user question
              5. Paragraphs: 2-4 detailed paragraphs with factual data from documents
              6. KeyPoints: 3-7 concise bullet points

              Example:
              {
                "Title": "Protein Requirements for Fat Loss",
                "Paragraphs": [
                  "During a cutting phase, protein intake should be 2.0–2.4g per kg of body weight."
                ],
                "KeyPoints": [
                  "Cutting phase: 2.0–2.4 g/kg body weight"
                ]
              }

              ═══════════════════════════════════════════════════════════════
              MALFORMED JSON TO FIX:
              ═══════════════════════════════════════════════════════════════
              {{malformedJson}}

              Output corrected JSON (first character '{', last character '}'):
              """;

        var response = await _chatCompletionService.GetChatMessageContentAsync(systemPrompt, new OpenAIPromptExecutionSettings
        {
            ChatSystemPrompt = systemPrompt,
            MaxTokens = responseTokensLimit,
            Temperature = 0.5f,
            ResponseFormat = ChatResponseFormat.ForJsonSchema<StructuredResponse>()
            // WebSearchOptions = null
            // ReasoningEffort = ChatReasoningEffortLevel.Medium
            // ReasoningEffort = "medium"
        }, cancellationToken: cancellationToken);

        var responseText = response.Content;

        var repairedJson = JsonSerializer.Deserialize<StructuredResponse>(responseText!)!;

        return repairedJson;
    }
}