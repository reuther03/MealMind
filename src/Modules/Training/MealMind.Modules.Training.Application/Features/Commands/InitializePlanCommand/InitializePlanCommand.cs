using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.Training.Application.Features.Commands.InitializePlanCommand;

public record InitializePlanCommand : ICommand<Guid>
{

    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;

    public InitializePlanCommand(IUserService userService, IUnitOfWork unitOfWork)
    {
        _userService = userService;
        _unitOfWork = unitOfWork;
    }

    public sealed class Handler : ICommandHandler<InitializePlanCommand, Guid>
    {
        public Task<Result<Guid>> Handle(InitializePlanCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}