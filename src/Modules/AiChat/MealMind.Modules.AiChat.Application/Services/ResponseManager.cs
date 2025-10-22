using System.Text.Json;
using System.Text.RegularExpressions;
using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Modules.AiChat.Application.Dtos;
using Microsoft.Extensions.AI;

namespace MealMind.Modules.AiChat.Application.Services;

internal sealed class ResponseManager : IResponseManager
{
    private const float TemperatureSetting = 0.0f;
    private const int MaxTokensSetting = 800;

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
            You are a nutrition assistant. Follow these rules EXACTLY:

            CONTENT RULES:
            1. Use ONLY information from the documents below
            2. Quote specific numbers, facts, and recommendations from documents
            3. If answer not in documents, respond: {{"Title":"Information Not Available","Paragraphs":["I don't have that information in my knowledge base."],"KeyPoints":[],"Sources":[]}}
            4. DO NOT use generic placeholder text like "This is a summary"
            5. DO NOT make up information

            OUTPUT FORMAT - CRITICAL:
            Return ONLY valid JSON matching this EXACT schema:
            {{"Title":"specific title from content","Paragraphs":["actual paragraph 1","actual paragraph 2"],"KeyPoints":["actual point 1","actual point 2","actual point 3"],"Sources":["document name 1","document
            name 2"]}}

            VALIDATION CHECKLIST:
            ✓ First character is {{
            ✓ Last character is }}
            ✓ No markdown fences (no ```json)
            ✓ Title is specific (not "This is a summary")
            ✓ Paragraphs contain actual facts from documents
            ✓ KeyPoints contain specific information
            ✓ Sources list document names you used

            EXAMPLE OF CORRECT OUTPUT:
            {{"Title":"Protein Requirements for Muscle Gain","Paragraphs":["For muscle gain and hypertrophy training, protein intake should be 1.6-2.2 grams per kilogram of body weight according to the Basic
            Nutrition Guidelines.","This higher protein intake supports muscle protein synthesis and recovery after resistance training."],"KeyPoints":["Aim for 1.6-2.2 g/kg for muscle gain","Spread protein across
            3-5 meals daily","Prefer complete protein sources like eggs, meat, fish"],"Sources":["Basic Nutrition Guidelines"]}}

            NOW ANSWER THE USER'S QUESTION USING THE DOCUMENTS:
            """;

        var systemDocumentsPrompt =
            $"""
             You are a nutrition assistant. Answer using ONLY the information in these documents.

             RULES:
             - Base your answer STRICTLY on the documents below
             - If the answer is not in the documents, say "I don't have that information" and nothing else
             - Quote specific numbers and facts from the documents or even quote them directly as reference below your answer
             - Do NOT use your general knowledge or make assumptions beyond the documents
             - Sources must be actual document titles provided below, at the start of each chunk look for Title: "Actual Document Title"

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