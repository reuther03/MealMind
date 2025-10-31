using System.Net.Http.Json;
using System.Text.Json;
using MealMind.Modules.AiChat.Application;
using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Modules.AiChat.Application.Dtos;
using MealMind.Modules.AiChat.Application.Options;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace MealMind.Modules.AiChat.Infrastructure.Services;

public class AiChatService : IAiChatService
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<OpenRouterModelOptions> _openRouterModelOptions;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private static readonly string[] Value = ["title", "paragraphs", "keyPoints", "sources"];

    public AiChatService(HttpClient httpClient, IOptions<OpenRouterModelOptions> openRouterModelOptions)
    {
        _httpClient = httpClient;
        _openRouterModelOptions = openRouterModelOptions;
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

        var response = await _httpClient.PostAsJsonAsync("chat/completions", new
        {
            model = "deepseek/deepseek-r1:free",
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = systemPrompt
                },

                new
                {
                    role = "user",
                    content = userPrompt
                }
            },
            response_format = new
            {
                type = "json_schema",
                json_schema = new
                {
                    name = "StructuredResponse",
                    strict = true,
                    schema = new
                    {
                        type = "object",
                        properties = new
                        {
                            title = new
                            {
                                type = "string",
                                description = "The title of the response base on generated response context."
                            },
                            paragraphs = new
                            {
                                type = "list",
                                description = "A list of paragraphs providing detailed information in response to the user's prompt sectioned appropriately."
                            },
                            keyPoints = new
                            {
                                type = "list", items = new { type = "string" },
                                description = "A list of key points summarizing the main ideas from the response."
                            },
                            sources = new
                            {
                                type = "list", items = new { type = "string" },
                                description = "A list of document titles that were used as sources for generating the response."
                            }
                        },
                        required = Value,
                        addidionalProperties = false
                    }
                }
            },
            max_tokens = responseTokensLimit,
        }, cancellationToken);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!content.StartsWith('{') ||
            !content.EndsWith('}'))
        {
            var fixMessage =
                $$"""
                  You are a JSON correction assistant. Your task is to fix malformed JSON responses based on the user's original question and reference documents.

                  ═══════════════════════════════════════════════════════════════
                  📚 REFERENCE DOCUMENTS
                  ═══════════════════════════════════════════════════════════════
                  {{documentsText}}

                  ═══════════════════════════════════════════════════════════════
                  🧩 TASK
                  ═══════════════════════════════════════════════════════════════
                  The user asked: "{{userPrompt}}"

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
                  {{content}}

                  Now output your corrected JSON but nothing else changing the content:
                  """;

            var fixedResponse = await _httpClient.PostAsJsonAsync("/chat/completions", new
            {
                model = _openRouterModelOptions.Value.BaseModel,
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = fixMessage
                    },

                    new
                    {
                        role = "user",
                        content = userPrompt
                    }
                },
                response_format = new
                {
                    type = "json_schema",
                    json_schema = new
                    {
                        name = "StructuredResponse",
                        strict = true,
                        schema = new
                        {
                            type = "object",
                            properties = new
                            {
                                title = new
                                {
                                    type = "string",
                                    description = "The title of the response base on generated response context."
                                },
                                paragraphs = new
                                {
                                    type = "list",
                                    description =
                                        "A list of paragraphs providing detailed information in response to the user's prompt sectioned appropriately."
                                },
                                keyPoints = new
                                {
                                    type = "list", items = new { type = "string" },
                                    description = "A list of key points summarizing the main ideas from the response."
                                },
                                sources = new
                                {
                                    type = "list", items = new { type = "string" },
                                    description = "A list of document titles that were used as sources for generating the response."
                                }
                            },
                            required = Value,
                            addidionalProperties = false
                        }
                    }
                },
                max_tokens = responseTokensLimit,
            }, cancellationToken);
        }

        var chatResponse = JsonSerializer.Deserialize<StructuredResponse>(content, _jsonSerializerOptions)!;

        return chatResponse;
    }
}