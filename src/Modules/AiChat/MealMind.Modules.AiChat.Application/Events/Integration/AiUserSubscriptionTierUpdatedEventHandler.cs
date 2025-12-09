using MealMind.Modules.AiChat.Application.Abstractions;
using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Shared.Abstractions.Events.Core;
using MealMind.Shared.Events.Identity;

namespace MealMind.Modules.AiChat.Application.Events.Integration;

public class AiUserSubscriptionTierUpdatedEventHandler : IEventHandler<SubscriptionTierUpdatedEvent>
{
    private readonly IAiChatUserRepository _aiChatUserRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AiUserSubscriptionTierUpdatedEventHandler(IAiChatUserRepository aiChatUserRepository, IUnitOfWork unitOfWork)
    {
        _aiChatUserRepository = aiChatUserRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SubscriptionTierUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var aiChatUser = await _aiChatUserRepository.GetByUserIdAsync(notification.UserId, cancellationToken);
        if (aiChatUser is null)
            return;

        aiChatUser.ChangeTier(notification.SubscriptionTier);
        _aiChatUserRepository.Update(aiChatUser);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}