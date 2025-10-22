using System.Text.Json;
using System.Text.RegularExpressions;
using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Modules.AiChat.Application.Dtos;
using Microsoft.Extensions.AI;

namespace MealMind.Modules.AiChat.Application.Services;

internal sealed class ResponseManager : IResponseManager
{
    private const float TemperatureSetting = 0.2f;
    private const int MaxTokensSetting = 900;

    private readonly IChatClient _chatClient;

    public ResponseManager(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<StructuredResponse> GenerateStructuredResponseAsync(string userPrompt, string documentsText, List<ChatMessage> chatMessages,
        CancellationToken cancellationToken = default)
    {
        const string systemPrompt =
            """
             You are a JSON-only response bot. You MUST respond with valid JSON and nothing else.

             STRICT RULES:
             1. Output ONLY raw JSON - NO markdown code blocks (no ```json or ```)
             2. NO explanatory text before or after the JSON
             3. NO additional keys beyond those specified
             4. ALL fields are REQUIRED (use empty arrays if needed)
             5. Follow the exact field names and types

             JSON SCHEMA (exact format):
             {
               "Title": "string - max 100 chars, single sentence summary",
               "Paragraphs": ["array of 2-5 strings", "each paragraph 50-150 words"],
               "KeyPoints": ["array of 3-7 strings", "each point is one short sentence"],
               "Sources": ["array of 0-5 strings", "document titles or references only"]
             }

             EXAMPLES OF CORRECT OUTPUT:
             {"Title":"Protein Requirements","Paragraphs":["First paragraph...","Second paragraph..."],"KeyPoints":["Point 1","Point 2"],"Sources":["Basic Nutrition Guidelines"]}

             INVALID OUTPUTS (DO NOT DO THIS):
                ```json {...}```
                Here is the answer: {...}
                Adding extra fields like "Summary" or "Notes"
                null values (use [] for empty arrays)

             YOUR RESPONSE MUST START WITH { AND END WITH }
            """;

        var systemDocumentsPrompt =
            $"""
             You are a nutrition assistant. Answer using ONLY the information in these documents.

             RULES:
             - Base your answer STRICTLY on the documents below
             - If the answer is not in the documents, say "I don't have that information" and nothing else
             - Quote specific numbers and facts from the documents or even quote them directly as reference below your answer
             - Do NOT use your general knowledge or make assumptions beyond the documents

             DOCUMENTS:
             {documentsText}
             """;

        var systemMessage = new ChatMessage(ChatRole.System, systemPrompt);
        var documentsMessage = new ChatMessage(ChatRole.System, systemDocumentsPrompt);

        chatMessages.Add(systemMessage);
        chatMessages.Add(documentsMessage);

        var userMessage = new ChatMessage(ChatRole.User, userPrompt);
        chatMessages.Add(userMessage);

        var response = await _chatClient.GetResponseAsync(chatMessages, new ChatOptions
        {
            Temperature = TemperatureSetting,
            MaxOutputTokens = MaxTokensSetting
        }, cancellationToken);

        var json = response.Text;

        try
        {
            var structuredResponse = JsonSerializer.Deserialize<StructuredResponse>(json)!;

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
                MaxOutputTokens = MaxTokensSetting
            }, cancellationToken: cancellationToken);

            var repairedJsonText = repairJson.Text;

            return JsonSerializer.Deserialize<StructuredResponse>(repairedJsonText)!;
        }
    }
}