using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.AiChat.Domain.AiChatUser;

public class AiChatUser : AggregateRoot<UserId>
{
    public SubscriptionTier Tier { get; private set; }
    public int ActiveConversations { get; private set; } // unchecked
    public int ConversationsLimit { get; private set; } // checked
    public int ConversationsMessagesHistoryDaysLimit { get; private set; } // checked
    public int DocumentsLimit { get; private set; } // not implemented yet
    public int PromptTokensLimit { get; private set; } // checked
    public int ResponseTokensLimit { get; private set; } // checked
    public int DailyPromptsLimit { get; private set; } // checked
    public bool CanExportData { get; private set; } // idk how to check this
    public bool CanUseAdvancedPrompts { get; private set; } // not implemented yet
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
            200,
            200,
            10,
            false,
            false,
            DateTime.UtcNow);

    // updates not creation of the user
    public AiChatUser ChangeTier(SubscriptionTier tier) => tier switch
    {
        SubscriptionTier.Free => new AiChatUser(
            Id,
            tier,
            ActiveConversations,
            2,
            7,
            0,
            200,
            200,
            10,
            false,
            false,
            StartDate,
            EndDate
        ),

        SubscriptionTier.Standard => new AiChatUser(
            Id,
            tier,
            ActiveConversations,
            5,
            30,
            5,
            500,
            500,
            50,
            true,
            false,
            StartDate,
            EndDate
        ),

        SubscriptionTier.Premium => new AiChatUser(
            Id,
            tier,
            ActiveConversations,
            20,
            90,
            20,
            1000,
            1000,
            -1,
            true,
            true,
            StartDate,
            EndDate
        ),

        _ => this
    };
}