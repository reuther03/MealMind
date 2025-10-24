using MealMind.Modules.AiChat.Application.Abstractions;
using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Domain.AiChatUser;
using MealMind.Shared.Abstractions.Events.Integration;
using MealMind.Shared.Abstractions.QueriesAndCommands.Notifications;

namespace MealMind.Modules.AiChat.Application.Events.Integration;

public class SubscriptionTierAddedEventHandler : INotificationHandler<SubscriptionTierAddedEvent>
{
    private readonly IAiChatUserRepository _aiChatUserRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SubscriptionTierAddedEventHandler(IAiChatUserRepository aiChatUserRepository, IUnitOfWork unitOfWork)
    {
        _aiChatUserRepository = aiChatUserRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SubscriptionTierAddedEvent notification, CancellationToken cancellationToken)
    {
        var aiChatUser = AiChatUser.Create(notification.Id);

        await _aiChatUserRepository.AddAsync(aiChatUser, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}