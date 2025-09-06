using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Shared.Abstractions.Kernel.CommandValidators;
using MealMind.Shared.Abstractions.Kernel.Primitives.Result;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;

namespace MealMind.Modules.Nutrition.Application.Features.Commands;

public record AddNutritionTargetCommand() : ICommand<Guid>
{
    public sealed class Handler : ICommandHandler<AddNutritionTargetCommand, Guid>
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

        public async Task<Result<Guid>> Handle(AddNutritionTargetCommand command, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileRepository.GetByIdAsync(_userService.UserId, cancellationToken);
            NullValidator.ValidateNotNull(userProfile);

            throw new NotImplementedException();
        }
    }
}