using System.Text.Json;
using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Modules.AiChat.Application.Dtos;
using Microsoft.Extensions.AI;

namespace MealMind.Modules.AiChat.Application.Services;

internal sealed class ResponseManager : IResponseManager
{
    private const float TemperatureSetting = 0.2f;
    private const int MaxTokensSetting = 800;

    private readonly IChatClient _chatClient;

    public ResponseManager(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<StructuredResponse> GenerateStructuredResponseAsync(
        string userPrompt,
        string documentsText,
        List<string> documentTitles,
        List<ChatMessage> chatMessages,
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

        var systemMessage = new ChatMessage(ChatRole.System, systemPrompt);

        chatMessages.Add(systemMessage);

        var userMessage = new ChatMessage(ChatRole.User, userPrompt);
        chatMessages.Add(userMessage);

        var response = await _chatClient.GetResponseAsync(chatMessages, new ChatOptions
        {
            Temperature = TemperatureSetting,
            MaxOutputTokens = MaxTokensSetting,
            ResponseFormat = ChatResponseFormat.ForJsonSchema<StructuredResponse>()
        }, cancellationToken);

        try
        {
            var structuredResponse = JsonSerializer.Deserialize<StructuredResponse>(response.Text)!;

            // if (!response.Text.StartsWith('{') ||  !response.Text.EndsWith('}'))

            return structuredResponse;
        }
        catch
        {
            const string systemRepairPrompt =
                """
                 The following JSON is malformed. Fix it to be valid JSON that adheres to the specified schema.

                 RULES:
                 1. Output ONLY raw JSON - NO markdown code blocks
                 2. NO explanatory text before or after the JSON
                 3. ALL fields are REQUIRED
                 4. Follow the exact field names and types

                 JSON SCHEMA:
                 {
                   "Title": "string - max 100 chars, single sentence summary",
                   "Paragraphs": ["array of 2-5 strings", "each paragraph 50-150 words"],
                   "KeyPoints": ["array of 3-7 strings", "each point is one short sentence"],
                   "Sources": ["array of 0-5 strings", "document titles or references only"]
                 }

                 Here is the malformed JSON:
                """;

            var repairJson = await _chatClient.GetResponseAsync(systemRepairPrompt, new ChatOptions
            {
                Temperature = TemperatureSetting,
                MaxOutputTokens = MaxTokensSetting,
                ResponseFormat = ChatResponseFormat.ForJsonSchema<StructuredResponse>()
            }, cancellationToken: cancellationToken);

            var repairedJsonText = repairJson.Text;

            return JsonSerializer.Deserialize<StructuredResponse>(repairedJsonText)!;
        }
    }

    public Task<string> AttemptJsonCorrectionAsync(string originalQuestion, string malformedJson, string documentsText,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}