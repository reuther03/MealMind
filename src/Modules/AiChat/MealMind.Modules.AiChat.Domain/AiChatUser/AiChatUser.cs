using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.AiChat.Domain.AiChatUser;

public class AiChatUser : AggregateRoot<UserId>
{
    public SubscriptionTier Tier { get; private set; }
    public int ActiveConversations { get; private set; }
    public int DocumentsLimit { get; private set; }
    public int PromptTokensLimit { get; private set; }
    public int ResponseTokensLimit { get; private set; }

    public int DailyPromptsLimit { get; private set; }

    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    private AiChatUser()
    {
    }

    private AiChatUser(
        UserId id,
        SubscriptionTier tier,
        int activeConversations,
        int documentsLimit,
        int promptTokensLimit,
        int responseTokensLimit,
        int dailyPromptsLimit,
        DateTime startDate,
        DateTime? endDate) : base(id)
    {
        Tier = tier;
        ActiveConversations = activeConversations;
        DocumentsLimit = documentsLimit;
        PromptTokensLimit = promptTokensLimit;
        ResponseTokensLimit = responseTokensLimit;
        DailyPromptsLimit = dailyPromptsLimit;
        StartDate = startDate;
        EndDate = endDate;
    }
}