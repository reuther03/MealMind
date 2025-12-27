using MealMind.Modules.Identity.Application.Abstractions;
using MealMind.Modules.Identity.Application.Abstractions.Database;
using MealMind.Modules.Identity.Domain.IdentityUser;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Dto.Nutrition;
using MealMind.Shared.Contracts.Result;
using MealMind.Shared.Events.Identity;

namespace MealMind.Modules.Identity.Application.Features.Commands.SignUpCommand;

public record SignUpCommand(
    string Username,
    string Email,
    string InputPassword,
    PersonalDataPayload PersonalData,
    List<NutritionTargetPayload> NutritionTargets
) : ICommand<Guid>
{
    public sealed class Handler : ICommandHandler<SignUpCommand, Guid>
    {
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutboxService _outboxService;

        public Handler(IIdentityUserRepository identityUserRepository, IUnitOfWork unitOfWork, IOutboxService outboxService)
        {
            _identityUserRepository = identityUserRepository;
            _unitOfWork = unitOfWork;
            _outboxService = outboxService;
        }

        public async Task<Result<Guid>> Handle(SignUpCommand command, CancellationToken cancellationToken)
        {
            if (await _identityUserRepository.ExistsWithEmailAsync(command.Email, cancellationToken))
                return Result<Guid>.BadRequest("Email already exists.");

            var identityUser = IdentityUser.Create(command.Username, command.Email, Password.Create(command.InputPassword));

            await _identityUserRepository.AddAsync(identityUser, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            await _outboxService.SaveAsync(
                new IdentityUserCreatedEvent(
                    identityUser.Id,
                    identityUser.Username,
                    identityUser.Email,
                    command.PersonalData,
                    command.NutritionTargets
                ), cancellationToken);

            await _outboxService.SaveAsync(
                new SubscriptionTierAddedEvent(
                    identityUser.Id
                ), cancellationToken);

            return Result.Ok(identityUser.Id.Value);
        }
    }
}