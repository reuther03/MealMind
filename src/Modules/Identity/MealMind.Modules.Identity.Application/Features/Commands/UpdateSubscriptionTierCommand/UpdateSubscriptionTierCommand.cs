using MealMind.Modules.Identity.Application.Abstractions;
using MealMind.Modules.Identity.Application.Abstractions.Database;
using MealMind.Shared.Abstractions.Kernel.CommandValidators;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.Identity.Application.Features.Commands.UpdateSubscriptionTierCommand;

public record UpdateSubscriptionTierCommand(Guid UserId, SubscriptionTier SubscriptionTier, string StripeCustomerId, string StripeSubscriptionId)
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
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            NullValidator.ValidateNotNull(user);

            user.UpdateSubscriptionTier(request.SubscriptionTier);
            user.Subscription.SetStripeDetails(request.StripeCustomerId, request.StripeSubscriptionId);

            await _unitOfWork.CommitAsync(cancellationToken);

            // event here

            return Result.Ok(user.Id.Value);
        }
    }
}