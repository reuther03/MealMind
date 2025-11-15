using MealMind.Modules.Identity.Application.Abstractions;
using MealMind.Modules.Identity.Application.Abstractions.Database;
using MealMind.Modules.Identity.Application.Features.Payloads;
using MealMind.Shared.Abstractions.Events.Integration;
using MealMind.Shared.Abstractions.Kernel.CommandValidators;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.Identity.Application.Features.Commands.Stripe.UpdateSubscriptionTierCommand;

public record UpdateSubscriptionTierCommand(UpdateSubscriptionTierPayload Payload)
    : ICommand<Guid>
{
    public sealed class Handler : ICommandHandler<UpdateSubscriptionTierCommand, Guid>
    {
        private readonly IIdentityUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublisher _publisher;

        public Handler(IIdentityUserRepository userRepository, IUnitOfWork unitOfWork, IPublisher publisher)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _publisher = publisher;
        }

        public async Task<Result<Guid>> Handle(UpdateSubscriptionTierCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Payload.UserId, cancellationToken);
            NullValidator.ValidateNotNull(user);

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

            await _publisher.Publish(
                new SubscriptionTierUpdatedEvent(
                user.Id,
                request.Payload.SubscriptionTier), cancellationToken);

            return Result.Ok(user.Id.Value);
        }
    }
}