using MealMind.Modules.AiChat.Application.Abstractions;
using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.AiChat.Application.Features.Commands.CreateFoodWithAiCommand;

public record CreateFoodWithAiCommand : ICommand<Guid>
{
    public sealed class Handler : ICommandHandler<CreateFoodWithAiCommand, Guid>
    {
        private readonly IAiChatUserRepository _userRepository;
        private readonly IAiChatService _aiChatService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public Task<Result<Guid>> Handle(CreateFoodWithAiCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}