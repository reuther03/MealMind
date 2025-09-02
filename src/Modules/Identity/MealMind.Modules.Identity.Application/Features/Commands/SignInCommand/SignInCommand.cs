using MealMind.Modules.Identity.Application.Abstractions.Database;
using MealMind.Shared.Abstractions.Auth;
using MealMind.Shared.Abstractions.Kernel.CommandValidators;
using MealMind.Shared.Abstractions.Kernel.Primitives.Result;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;

namespace MealMind.Modules.Identity.Application.Features.Commands.SignInCommand;

public sealed record SignInCommand(string Email, string Password) : ICommand<AccessToken>
{
    public sealed class Handler : ICommandHandler<SignInCommand, AccessToken>
    {
        private readonly IIdentityUserRepository _userRepository;
        private readonly IJwtProvider _jwtProvider;

        public Handler(IIdentityUserRepository userRepository, IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
        }

        public async Task<Result<AccessToken>> Handle(SignInCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);
            NullValidator.ValidateNotNull(user);

            if (!user.Password.Verify(command.Password))
                return Result<AccessToken>.BadRequest("Invalid credentials");

            var accessToken = AccessToken.Create(
                _jwtProvider.GenerateToken(user.Id.ToString(), user.Email, user.Username),
                user.Id,
                user.Email,
                user.Username);

            return Result<AccessToken>.Ok(accessToken);
        }
    }
}