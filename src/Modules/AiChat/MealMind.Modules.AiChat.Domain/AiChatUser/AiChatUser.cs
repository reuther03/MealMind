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
    public int DailyImageAnalysisLimit { get; private set; }
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
        int dailyImageAnalysisLimit,
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
        DailyImageAnalysisLimit = dailyImageAnalysisLimit;
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
            200,
            200,
            10,
            false,
            false,
            0,
            DateTime.UtcNow);

    public void IncrementActiveConversations()
    {
        if (ActiveConversations >= ConversationsLimit)
            throw new ApplicationException("Conversations limit exceeded.");

        ActiveConversations++;
    }


    public void ChangeTier(SubscriptionTier tier)
    {
        Tier = tier;
        switch (tier)
        {
            case SubscriptionTier.Free:
                Tier = tier;
                ConversationsLimit = 2;
                ConversationsMessagesHistoryDaysLimit = 7;
                DocumentsLimit = 1;
                PromptTokensLimit = 200;
                ResponseTokensLimit = 200;
                DailyPromptsLimit = 10;
                CanExportData = false;
                CanUseAdvancedPrompts = false;
                DailyImageAnalysisLimit = 0;
                break;
            case SubscriptionTier.Standard:
                Tier = tier;
                ConversationsLimit = 5;
                ConversationsMessagesHistoryDaysLimit = 30;
                DocumentsLimit = 5;
                PromptTokensLimit = 500;
                ResponseTokensLimit = 500;
                DailyPromptsLimit = 50;
                CanExportData = true;
                CanUseAdvancedPrompts = false;
                DailyImageAnalysisLimit = 3;
                break;
            case SubscriptionTier.Premium:
                Tier = tier;
                ConversationsLimit = 20;
                ConversationsMessagesHistoryDaysLimit = 90;
                DocumentsLimit = 20;
                PromptTokensLimit = 1000;
                ResponseTokensLimit = 1000;
                DailyPromptsLimit = -1;
                CanExportData = true;
                CanUseAdvancedPrompts = true;
                DailyImageAnalysisLimit = 10;
                break;
            default:
                return;
        }

        StartDate = DateTime.UtcNow;
    }
}