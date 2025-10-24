using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.AiChat.Domain.User;

public class SubscriptionPlan : Entity<Guid>
{
    public UserId UserId { get; private set; }
    public int ActiveConversations { get; private set; }
    public int DocumentsLimit { get; private set; }
    public int PromptTokensLimit { get; private set; }
    public int ResponseTokensLimit { get; private set; }

    public int DailyPromptsLimit { get; private set; }

    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
}