using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text.Json;
using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Modules.AiChat.Application.Dtos;
using MealMind.Modules.AiChat.Application.Options;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI.Chat;
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