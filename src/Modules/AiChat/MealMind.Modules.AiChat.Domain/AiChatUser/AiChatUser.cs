using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.AiChat.Domain.AiChatUser;

public class AiChatUser : AggregateRoot<UserId>
{
    public SubscriptionTier Tier { get; private set; }
    public int ActiveConversations { get; private set; }
    public int ConversationsLimit { get; private set; }
    public int ConversationsMessagesHistoryDaysLimit { get; private set; }
    public int DocumentsLimit { get; private set; }
    public int PromptTokensLimit { get; private set; }
    public int ResponseTokensLimit { get; private set; }
    public int DailyPromptsLimit { get; private set; }
    public bool CanExportData { get; private set; }
    public bool CanUseAdvancedPrompts { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    private AiChatUser()
    {
    }

    private AiChatUser(
        UserId id,
        SubscriptionTier tier,
        int activeConversations,
        int conversationsLimit,
        int conversationsMessagesHistoryDaysLimit,
        int documentsLimit,
        int promptTokensLimit,
        int responseTokensLimit,
        int dailyPromptsLimit,
        bool canExportData,
        bool canUseAdvancedPrompts,
        DateTime startDate,
        DateTime? endDate = null)
        : base(id)
    {
        Tier = tier;
        ActiveConversations = activeConversations;
        ConversationsLimit = conversationsLimit;
        ConversationsMessagesHistoryDaysLimit = conversationsMessagesHistoryDaysLimit;
        DocumentsLimit = documentsLimit;
        PromptTokensLimit = promptTokensLimit;
        ResponseTokensLimit = responseTokensLimit;
        DailyPromptsLimit = dailyPromptsLimit;
        CanExportData = canExportData;
        CanUseAdvancedPrompts = canUseAdvancedPrompts;
        StartDate = startDate;
        EndDate = endDate;
    }

    public static AiChatUser Create(UserId userId)
        => new(
            userId,
            SubscriptionTier.Free,
            0,
            2,
            7,
            1,
            250,
            300,
            10,
            false,
            false,
            DateTime.UtcNow);
}