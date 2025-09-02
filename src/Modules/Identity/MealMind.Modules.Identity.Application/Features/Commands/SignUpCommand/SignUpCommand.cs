using MealMind.Modules.Identity.Application.Abstractions;
using MealMind.Modules.Identity.Application.Abstractions.Database;
using MealMind.Modules.Identity.Domain.IdentityUser;
using MealMind.Shared.Abstractions.Kernel.Primitives.Result;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;

namespace MealMind.Modules.Identity.Application.Features.Commands.SignUpCommand;

public record SignUpCommand(string Username, string Email, string InputPassword) : ICommand<Guid>
{
    public sealed class Handler : ICommandHandler<SignUpCommand, Guid>
    {
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IIdentityUserRepository identityUserRepository, IUnitOfWork unitOfWork)
        {
            _identityUserRepository = identityUserRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(SignUpCommand command, CancellationToken cancellationToken)
        {
            if (await _identityUserRepository.ExistsWithEmailAsync(command.Email, cancellationToken))
                return Result<Guid>.BadRequest("Email already exists.");

            var identityUser = IdentityUser.Create(command.Username, command.Email, Password.Create(command.InputPassword));

            await _identityUserRepository.AddAsync(identityUser, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.Ok(identityUser.Id.Value);
        }
    }
}