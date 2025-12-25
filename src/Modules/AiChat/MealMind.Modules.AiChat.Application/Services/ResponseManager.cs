using System.Text.Json;
using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Modules.AiChat.Application.Dtos;
using MealMind.Shared.Contracts.Dto.AiChat;
using Microsoft.Extensions.AI;

namespace MealMind.Modules.AiChat.Application.Services;

internal sealed class ResponseManager : IResponseManager
{
    private const float TemperatureSetting = 0.2f;

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
        int responseTokensLimit,
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
            MaxOutputTokens = responseTokensLimit,
            ResponseFormat = ChatResponseFormat.ForJsonSchema<StructuredResponse>()
        }, cancellationToken);

        if (!response.Text.StartsWith('{') ||
            !response.Text.EndsWith('}'))
        {
            var repairedJson =
                await AttemptJsonCorrectionAsync(userPrompt, response.Text, documentsText, documentTitles, responseTokensLimit, cancellationToken);

            return repairedJson;
        }

        var structuredResponse = JsonSerializer.Deserialize<StructuredResponse>(response.Text)!;

        // thing about 2 phase reasoning with finally block
        return structuredResponse;
    }

    private async Task<StructuredResponse> AttemptJsonCorrectionAsync(string originalQuestion, string malformedJson, string documentsText,
        List<string> documentTitles, int responseTokensLimit,
        CancellationToken cancellationToken = default)
    {
        var systemPrompt =
            $$"""
              You are a JSON correction assistant. Your task is to fix malformed JSON responses based on the user's original question and reference documents.

              ═══════════════════════════════════════════════════════════════
              📚 REFERENCE DOCUMENTS
              ═══════════════════════════════════════════════════════════════
              {{documentsText}}

              ═══════════════════════════════════════════════════════════════
              🧩 TASK
              ═══════════════════════════════════════════════════════════════
              The user asked: "{{originalQuestion}}"

              The following JSON response is malformed. Correct it to be valid JSON that adheres to the specified schema.

              RULES:
              1. Output ONLY raw JSON - NO markdown code blocks
              2. NO explanatory text before or after the JSON
              3. ALL fields are REQUIRED
              4. Follow the exact field names and types

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
                ["{{documentTitles}}"]

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


              Here is the malformed JSON:
              {{malformedJson}}

              Now output your corrected JSON but nothing else changing the content:
              """;

        var repairJson = await _chatClient.GetResponseAsync(systemPrompt, new ChatOptions
        {
            Temperature = TemperatureSetting,
            MaxOutputTokens = responseTokensLimit,
            ResponseFormat = ChatResponseFormat.ForJsonSchema<StructuredResponse>()
        }, cancellationToken: cancellationToken);

        var repairedJson = JsonSerializer.Deserialize<StructuredResponse>(repairJson.Text)!;

        return repairedJson;
    }
}