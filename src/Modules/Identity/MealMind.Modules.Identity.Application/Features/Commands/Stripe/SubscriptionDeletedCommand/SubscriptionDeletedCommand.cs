using MealMind.Modules.Identity.Application.Abstractions;
using MealMind.Modules.Identity.Application.Abstractions.Database;
using MealMind.Shared.Abstractions.Kernel.CommandValidators;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.Identity.Application.Features.Commands.Stripe.SubscriptionDeletedCommand;

public record SubscriptionDeletedCommand(string CustomerId, DateTime? CanceledAt, string Status) : ICommand<bool>
{
    public sealed class Handler : ICommandHandler<SubscriptionDeletedCommand, bool>
    {
        private readonly IIdentityUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IIdentityUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(SubscriptionDeletedCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByCustomerIdAsync(command.CustomerId, cancellationToken);
            Validator.ValidateNotNull(user);

            var updatedSubscription =
                user.Subscription.Cancel(user.Subscription.StripeCustomerId!, command.CanceledAt, command.Status);

            user.UpdateSubscription(updatedSubscription);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.Ok(true);
        }
    }
}