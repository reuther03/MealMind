using MealMind.Modules.Identity.Application.Abstractions;
using MealMind.Modules.Identity.Application.Abstractions.Database;
using MealMind.Modules.Identity.Application.Features.Payloads;
using MealMind.Shared.Abstractions.Kernel.CommandValidators;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.Identity.Application.Features.Commands.UpdateSubscriptionTierCommand;

public record UpdateSubscriptionTierCommand(UpdateSubscriptionTierPayload Payload)
    : ICommand<Guid>
{
    public sealed class Handler : ICommandHandler<UpdateSubscriptionTierCommand, Guid>
    {
        private readonly IIdentityUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IIdentityUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
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

            // event here

            return Result.Ok(user.Id.Value);
        }
    }
}