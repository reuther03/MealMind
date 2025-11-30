using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.Nutrition.Application.Features.Commands.UpdateDailyLogUserWeight;

public record UpdateDailyLogUserWeight(DateOnly DailyLogDate, decimal CurrentWeight) : ICommand<bool>
{
    public sealed class Handler : ICommandHandler<UpdateDailyLogUserWeight, bool>
    {
        private readonly IDailyLogRepository _dailyLogRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IDailyLogRepository dailyLogRepository, IUserProfileRepository userProfileRepository, IUserService userService, IUnitOfWork unitOfWork)
        {
            _dailyLogRepository = dailyLogRepository;
            _userProfileRepository = userProfileRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateDailyLogUserWeight command, CancellationToken cancellationToken)
        {
            var user = await _userProfileRepository.GetByIdAsync(_userService.UserId, cancellationToken);
            if (user is null)
                return Result<bool>.BadRequest("User profile not found.");

            var dailyLog = await _dailyLogRepository.GetByDateAsync(command.DailyLogDate, user.Id, cancellationToken);
            if (dailyLog is null)
                return Result<bool>.BadRequest("Daily log not found.");

            dailyLog.UpdateCurrentWeight(command.CurrentWeight);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<bool>.Ok(true);
        }
    }
}