using MealMind.Modules.Identity.Application.Abstractions;
using MealMind.Modules.Identity.Application.Abstractions.Database;
using MealMind.Shared.Abstractions.Kernel.CommandValidators;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.Identity.Application.Features.Commands.UpdateSubscriptionAfterPaymentCommand;

public record UpdateSubscriptionAfterPaymentCommand(string SubscriptionId, SubscriptionTier Tier, DateTime PeriodStart, DateTime PeriodEnd, string Status)
    : ICommand<bool>
{
    public sealed class Handler : ICommandHandler<UpdateSubscriptionAfterPaymentCommand, bool>
    {
        private readonly IIdentityUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IIdentityUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateSubscriptionAfterPaymentCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserBySubscriptionIdAsync(request.SubscriptionId, cancellationToken);
            NullValidator.ValidateNotNull(user);

            user.Subscription.EnsureTier(
                request.Tier,
                request.PeriodStart,
                request.PeriodEnd,
                request.Status);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.Ok(true);
        }
    }
}