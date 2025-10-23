using System.Text.Json;
using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Modules.AiChat.Application.Dtos;
using Microsoft.Extensions.AI;

namespace MealMind.Modules.AiChat.Application.Services;

internal sealed class ResponseManager : IResponseManager
{
    private const float TemperatureSetting = 0.1f;
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
              You are a nutrition knowledge assistant. Answer questions using ONLY the provided reference documents.

              ═══════════════════════════════════════════════════════════════
              📚 REFERENCE DOCUMENTS
              ═══════════════════════════════════════════════════════════════

              {{documentsText}}

              ═══════════════════════════════════════════════════════════════
              ✅ EXAMPLE: Learn from this pattern
              ═══════════════════════════════════════════════════════════════

              User asks: "How much protein for cutting?"

              You respond with THIS JSON (using facts from documents):

              {"Title":"Protein Requirements for Fat Loss Phase","Paragraphs":["During a cutting phase, protein intake should be 2.0-2.4 grams per kilogram of body weight to preserve lean muscle mass while in a calorie deficit.","This is higher than the muscle gain recommendation of 1.6-2.2 g/kg because protein helps prevent muscle breakdown when calories are restricted."],"KeyPoints":["Cutting phase: 2.0-2.4 g/kg body weight daily","Spread protein across 3-5 meals for best absorption","Use complete protein sources like eggs, meat, fish, dairy","Higher than muscle gain phase to preserve muscle mass"],"Sources":["Basic Nutrition Guidelines"]}

              Notice:
              - Title is SPECIFIC to the question (NOT "This is a summary")
              - Title should reference the CONTEXT of the question not be generic
              - Paragraphs contain ACTUAL FACTS with NUMBERS from documents (NOT "The first paragraph contains...")
              - Paragraphs are DETAILED and COMPREHENSIVE (100-250 words each)
              - Paragraphs include ALL relevant details: numbers, percentages, ranges, recommendations, explanations
              - KeyPoints are SHORT summaries (one sentence each, 10-30 words)
              - Sources use EXACT document titles

              PARAGRAPH vs KEY POINTS:
              ✓ Paragraphs = DETAILED explanations with context, numbers, reasoning
              ✓ KeyPoints = BRIEF bullet summaries of main takeaways
              ✓ Paragraphs should be MUCH LONGER than KeyPoints

              ═══════════════════════════════════════════════════════════════
              🎯 YOUR TASK
              ═══════════════════════════════════════════════════════════════

              Answer the user's question the SAME WAY as the example above:

              1. Extract ALL SPECIFIC facts, numbers, recommendations from documents
              2. Create a DESCRIPTIVE title related to the question
              3. Write 2-4 DETAILED paragraphs (100-250 words each):
                 - Include specific numbers, ranges, percentages from documents
                 - Explain WHY and HOW (mechanisms, reasons, context)
                 - Cover multiple aspects of the topic
                 - Use transitional phrases between ideas
              4. List 3-7 SHORT key points (one sentence each):
                 - Each point = ONE main takeaway in 10-30 words
                 - Summarize the detailed info from paragraphs
                 - Do NOT add new details here
              5. Include EXACT document titles you used

              Available document titles (use these EXACTLY in Sources field):
              ["{{availableSourcesList}}"]

              CRITICAL:
              - Use ONLY information directly stated in the documents above
              - Extract EVERY relevant detail from documents into paragraphs
              - If answer NOT in documents: {"Title":"Information Not Available","Paragraphs":["I don't have that information in my knowledge base."],"KeyPoints":[],"Sources":[]}
              - Quote specific numbers and details (e.g., "1.6-2.2 g/kg", "7-9 hours")
              - Do NOT make up sources or use titles not in the list
              - Document titles are shown above each chunk in REFERENCE DOCUMENTS section

              ═══════════════════════════════════════════════════════════════
              ❌ WRONG EXAMPLE (Do NOT copy this)
              ═══════════════════════════════════════════════════════════════

              {"Title":"This is a summary","Paragraphs":["The first paragraph contains information about the topic.","The second paragraph provides additional details."],"KeyPoints":["Point 1: This is a short sentence.","Point 2: Another short sentence."],"Sources":["Source 1","Source 2"]}

              This is WRONG because it uses placeholder text instead of actual facts from documents.

              ═══════════════════════════════════════════════════════════════
              🚨 FORMAT REQUIREMENTS
              ═══════════════════════════════════════════════════════════════

              1. First character MUST be {
              2. Last character MUST be }
              3. NO markdown fences (```json is forbidden)
              4. NO explanatory text before or after JSON
              5. Sources MUST be exact titles from the list above

              ═══════════════════════════════════════════════════════════════

              Now answer the user's question with a JSON response containing ACTUAL INFORMATION from the documents:
              """;

        var systemMessage = new ChatMessage(ChatRole.System, systemPrompt);

        chatMessages.Add(systemMessage);

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

    public Task<string> AttemptJsonCorrectionAsync(string originalQuestion, string malformedJson, string documentsText,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}