using MealMind.Modules.Identity.Application.Abstractions;
using MealMind.Modules.Identity.Application.Abstractions.Database;
using MealMind.Shared.Abstractions.Kernel.CommandValidators;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.Identity.Application.Features.Commands.Stripe.StripeSubscriptionTierChangesCommand;

public record SubscriptionTierChangesCommand(
    SubscriptionTier SubscriptionTier,
    string StripeCustomerId,
    string StripeSubscriptionId,
    DateTime CurrentPeriodStart,
    DateTime CurrentPeriodEnd,
    string SubscriptionStatus) : ICommand<bool>
{
    public sealed class Handler : ICommandHandler<SubscriptionTierChangesCommand, bool>
    {
        private readonly IIdentityUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IIdentityUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(SubscriptionTierChangesCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByCustomerIdAsync(command.StripeCustomerId, cancellationToken);
            Validator.ValidateNotNull(user);

            var updatedSubscription = user.Subscription.UpdateToPaidTier(
                command.SubscriptionTier,
                command.StripeCustomerId,
                command.StripeSubscriptionId,
                user.Subscription.SubscriptionStartedAt,
                command.CurrentPeriodStart,
                command.CurrentPeriodEnd,
                command.SubscriptionStatus);

            user.UpdateSubscription(updatedSubscription);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.Ok(true);
        }
    }
}