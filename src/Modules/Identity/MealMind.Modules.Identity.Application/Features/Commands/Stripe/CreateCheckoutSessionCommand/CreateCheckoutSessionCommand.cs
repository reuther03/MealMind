using MealMind.Modules.Identity.Application.Abstractions;
using MealMind.Modules.Identity.Application.Abstractions.Database;
using MealMind.Modules.Identity.Application.Abstractions.Services;
using MealMind.Shared.Abstractions.Kernel.CommandValidators;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.Identity.Application.Features.Commands.Stripe.CreateCheckoutSessionCommand;

public record CreateCheckoutSessionCommand(SubscriptionTier SubscriptionTier) : ICommand<string>
{
    public sealed class Handler : ICommandHandler<CreateCheckoutSessionCommand, string>
    {
        private readonly IStripeService _stripeService;
        private readonly IUserService _userService;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IStripeService stripeService, IUserService userService, IIdentityUserRepository identityUserRepository, IUnitOfWork unitOfWork)
        {
            _stripeService = stripeService;
            _userService = userService;
            _identityUserRepository = identityUserRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(CreateCheckoutSessionCommand command, CancellationToken cancellationToken)
        {
            var user = await _identityUserRepository.GetByIdAsync(_userService.UserId, cancellationToken);
            NullValidator.ValidateNotNull(user);

            var url = await _stripeService.CreateCheckoutSessionAsync(user.Id, command.SubscriptionTier);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.Ok(url);
        }
    }
}