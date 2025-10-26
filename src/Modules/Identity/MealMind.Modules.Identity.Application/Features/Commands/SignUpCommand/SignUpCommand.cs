using MealMind.Modules.Identity.Application.Abstractions;
using MealMind.Modules.Identity.Application.Abstractions.Database;
using MealMind.Modules.Identity.Domain.IdentityUser;
using MealMind.Shared.Abstractions.Events.Integration;
using MealMind.Shared.Abstractions.Kernel.Payloads;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.Identity.Application.Features.Commands.SignUpCommand;

public record SignUpCommand(
    string Username,
    string Email,
    string InputPassword,
    PersonalDataPayload PersonalData) : ICommand<Guid>
{
    public sealed class Handler : ICommandHandler<SignUpCommand, Guid>
    {
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublisher _publisher;

        public Handler(IIdentityUserRepository identityUserRepository, IUnitOfWork unitOfWork, IPublisher publisher)
        {
            _identityUserRepository = identityUserRepository;
            _unitOfWork = unitOfWork;
            _publisher = publisher;
        }

        public async Task<Result<Guid>> Handle(SignUpCommand command, CancellationToken cancellationToken)
        {
            if (await _identityUserRepository.ExistsWithEmailAsync(command.Email, cancellationToken))
                return Result<Guid>.BadRequest("Email already exists.");

            var identityUser = IdentityUser.Create(command.Username, command.Email, Password.Create(command.InputPassword));

            await _identityUserRepository.AddAsync(identityUser, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            await _publisher.Publish(
                new IdentityUserCreatedEvent(
                    identityUser.Id.Value,
                    identityUser.Username,
                    identityUser.Email,
                    command.PersonalData
                ), cancellationToken);

            await _publisher.Publish(
                new SubscriptionTierAddedEvent(
                    identityUser.Id,
                    identityUser.Tier
                ), cancellationToken);

            return Result.Ok(identityUser.Id.Value);
        }
    }
}