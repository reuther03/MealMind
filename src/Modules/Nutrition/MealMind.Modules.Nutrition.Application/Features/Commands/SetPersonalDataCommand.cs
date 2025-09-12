using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Domain.UserProfile;
using MealMind.Shared.Abstractions.Kernel.CommandValidators;
using MealMind.Shared.Abstractions.Kernel.Primitives.Result;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;

namespace MealMind.Modules.Nutrition.Application.Features.Commands;

public record SetPersonalDataCommand(Gender Gender, DateOnly DateOfBirth, decimal Weight, decimal Height, decimal WeightTarget, ActivityLevel ActivityLevel)
    : ICommand<bool>
{
    public sealed class Handler : ICommandHandler<SetPersonalDataCommand, bool>
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IUserProfileRepository userProfileRepository, IUserService userService, IUnitOfWork unitOfWork)
        {
            _userProfileRepository = userProfileRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(SetPersonalDataCommand command, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileRepository.GetByIdAsync(_userService.UserId, cancellationToken);
            NullValidator.ValidateNotNull(userProfile);

            var personalData = PersonalData.Create(
                command.Gender,
                command.DateOfBirth,
                command.Weight,
                command.Height,
                command.WeightTarget,
                command.ActivityLevel
            );

            userProfile.SetPersonalData(personalData);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<bool>.Ok(true);
        }
    }
}