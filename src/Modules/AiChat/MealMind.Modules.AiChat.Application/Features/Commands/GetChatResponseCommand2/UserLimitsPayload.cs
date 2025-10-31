namespace MealMind.Modules.AiChat.Application.Features.Commands.GetChatResponseCommand2;

public record UserLimitsPayload(
    int ConversationsLimit,
    int ConversationsMessagesHistoryDaysLimit,
    int DocumentsLimit,
    int PromptTokensLimit,
    int ResponseTokensLimit,
    int DailyPromptsLimit,
    bool CanExportData,
    bool CanUseAdvancedPrompts);