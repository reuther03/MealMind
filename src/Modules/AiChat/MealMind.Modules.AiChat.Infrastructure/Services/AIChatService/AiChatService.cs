using System.Text.Json;
using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Modules.AiChat.Application.Dtos;
using MealMind.Shared.Abstractions.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ChatMessageContent = Microsoft.SemanticKernel.ChatMessageContent;
using ChatResponseFormat = Microsoft.Extensions.AI.ChatResponseFormat;
using TextContent = Microsoft.SemanticKernel.TextContent;

namespace MealMind.Modules.AiChat.Infrastructure.Services.AIChatService;

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
        var systemPrompt = PromptTemplate.ConversationPrompt(userPrompt, documentsText);

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

        var systemMessage = new ChatMessageContent(AuthorRole.System, PromptTemplate.ImageAnalysisPrompt(userPrompt));

        var chatHistory = new ChatHistory();

        var complexUserMessage = new ChatMessageContent(AuthorRole.User, new ChatMessageContentItemCollection
        {
            new TextContent(userPrompt ?? string.Empty),
            new ImageContent(imageBytes, "image/jpeg")
        });

        chatHistory.AddRange([systemMessage, complexUserMessage]);

        var response = await _chatCompletionService.GetChatMessageContentsAsync(chatHistory, new GeminiPromptExecutionSettings
        {
            MaxTokens = 2000,
            Temperature = 0.3f,
            ThinkingConfig = new GeminiThinkingConfig { ThinkingBudget = 0 },
            ResponseMimeType = "application/json"
        }, cancellationToken: cancellationToken);

        var responseText = response[0].Content;

        if (string.IsNullOrWhiteSpace(responseText))
            throw new InvalidOperationException("Vision model returned empty response");

        var structuredResponse = JsonSerializer.Deserialize<AnalyzedImageStructuredResponse>(responseText, _jsonSerializerOptions)!;
        var structuredResponseWithImage = structuredResponse with { ImageBytes = imageBytes.ToArray() };

        return structuredResponseWithImage;
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