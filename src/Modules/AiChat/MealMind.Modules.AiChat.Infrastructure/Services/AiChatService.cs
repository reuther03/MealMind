using System.Net.Http.Json;
using System.Text.Json;
using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Modules.AiChat.Application.Dtos;
using MealMind.Modules.AiChat.Application.Options;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

namespace MealMind.Modules.AiChat.Infrastructure.Services;

public class AiChatService : IAiChatService
{
    private readonly IOptions<OpenRouterModelOptions> _openRouterModelOptions;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly IChatClient _chatClient;
    private static readonly string[] Value = ["title", "paragraphs", "keyPoints", "sources"];

    public AiChatService(IOptions<OpenRouterModelOptions> openRouterModelOptions, IChatClient chatClient)
    {
        _openRouterModelOptions = openRouterModelOptions;
        _chatClient = chatClient;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
    }

    public async Task<StructuredResponse> GenerateStructuredResponseAsync(string userPrompt, string documentsText, List<string> documentTitles,
        List<ChatMessage> chatMessages, int responseTokensLimit,
        CancellationToken cancellationToken = default)
    {
        var availableSourcesList = string.Join("\", \"", documentTitles);

        var systemPrompt =
            $$"""
              You are a nutrition knowledge assistant. Answer questions **using ONLY** the reference documents below. Never invent or summarize generically.

              ═══════════════════════════════════════════════════════════════
              📚 REFERENCE DOCUMENTS
              ═══════════════════════════════════════════════════════════════
              {{documentsText}}

              ═══════════════════════════════════════════════════════════════
              🎯 TASK
              ═══════════════════════════════════════════════════════════════
              Answer the user's question by extracting *factual, numeric, and explanatory details* from the documents. 
              Return ONLY valid JSON matching this exact schema:

              {
                "Title": "string",
                "Paragraphs": ["string", "string", ...],
                "KeyPoints": ["string", "string", ...],
                "Sources": ["string", "string", ...]
              }

              ═══════════════════════════════════════════════════════════════
              ✅ EXAMPLE OUTPUT
              ═══════════════════════════════════════════════════════════════
              User: "How much protein for cutting?"
              Assistant:
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
                ],
                "Sources": ["Basic Nutrition Guidelines"]
              }

              ═══════════════════════════════════════════════════════════════
              🧩 STRUCTURE REQUIREMENTS
              ═══════════════════════════════════════════════════════════════
              - **Title** → descriptive and specific to the user question.
              - **Paragraphs** → 2–4 detailed paragraphs (100–250 words each). Must include concrete data, ranges, or mechanisms.
              - **KeyPoints** → 3–7 concise one-sentence summaries of main facts (10–30 words).
              - **Sources** → exact document titles from:
                ["{{availableSourcesList}}"]

              If information is missing:
              {
                "Title": "Information Not Available",
                "Paragraphs": ["I don't have that information in my knowledge base."],
                "KeyPoints": [],
                "Sources": []
              }

              ═══════════════════════════════════════════════════════════════
              🚫 RESTRICTIONS
              ═══════════════════════════════════════════════════════════════
              - Do NOT create placeholder text (e.g. “The first paragraph contains…”).
              - Do NOT include generic summaries (“This document explains...”).
              - Do NOT output anything except JSON.
              - No ```json fences or markdown.
              - First character must be '{' and last character must be '}'.
              - Output must be valid, parseable JSON.

              ═══════════════════════════════════════════════════════════════
              ⚠️ FINAL REMINDER
              ═══════════════════════════════════════════════════════════════
              Before finishing, check:
              1. JSON is syntactically valid.
              2. All fields match schema.
              3. Every paragraph and key point uses *actual data* from documents.

              Now output your final answer in pure JSON:
              """;


        var response = await _chatClient.GetResponseAsync(systemPrompt, new ChatOptions
        {
            MaxOutputTokens = responseTokensLimit,
            ResponseFormat = ChatResponseFormat.ForJsonSchema<StructuredResponse>()
        }, cancellationToken);

        throw new NotImplementedException();
    }
}