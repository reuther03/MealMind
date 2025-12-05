using MealMind.Modules.Identity.Application.Abstractions;
using MealMind.Modules.Identity.Application.Abstractions.Database;
using MealMind.Modules.Identity.Application.Features.Payloads;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;
using MealMind.Shared.Events.Identity;

namespace MealMind.Modules.Identity.Application.Features.Commands.Stripe.UpdateSubscriptionTierCommand;

public record UpdateSubscriptionTierCommand(UpdateSubscriptionTierPayload Payload)
    : ICommand<Guid>
{
    public sealed class Handler : ICommandHandler<UpdateSubscriptionTierCommand, Guid>
    {
        private readonly IIdentityUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutboxService _outboxService;

        public Handler(IIdentityUserRepository userRepository, IUnitOfWork unitOfWork, IOutboxService outboxService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _outboxService = outboxService;
        }

        public async Task<Result<Guid>> Handle(UpdateSubscriptionTierCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Payload.UserId, cancellationToken);
            if (user is null)
                return Result<Guid>.NotFound("User not found.");

            var updatedSubscription = user.Subscription.UpdateToPaidTier(
                request.Payload.SubscriptionTier,
                request.Payload.StripeCustomerId,
                request.Payload.StripeSubscriptionId,
                request.Payload.SubscriptionStartedAt,
                request.Payload.CurrentPeriodStart,
                request.Payload.CurrentPeriodEnd,
                request.Payload.SubscriptionStatus);

            user.UpdateSubscription(updatedSubscription);

            await _unitOfWork.CommitAsync(cancellationToken);

            await _outboxService.SaveAsync(
                new SubscriptionTierUpdatedEvent(
                    user.Id,
                    request.Payload.SubscriptionTier), cancellationToken);

            return Result.Ok(user.Id.Value);
        }
    }
}