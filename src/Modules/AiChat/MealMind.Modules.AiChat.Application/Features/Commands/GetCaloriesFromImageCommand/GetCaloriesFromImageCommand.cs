using MealMind.Modules.AiChat.Application.Abstractions;
using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Shared.Abstractions.Kernel.CommandValidators;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;
using Microsoft.AspNetCore.Http;

namespace MealMind.Modules.AiChat.Application.Features.Commands.GetCaloriesFromImageCommand;

public record GetCaloriesFromImageCommand(string Prompt, IFormFile Image) : ICommand<string>
{
    public sealed class Handler : ICommandHandler<GetCaloriesFromImageCommand, string>
    {
        private readonly IAiChatUserRepository _userRepository;
        private readonly IAiChatService _aiChatService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;


        public Handler(IAiChatUserRepository userRepository, IAiChatService aiChatService, IUserService userService, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _aiChatService = aiChatService;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(GetCaloriesFromImageCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUserIdAsync(_userService.UserId, cancellationToken);
            Validator.ValidateNotNull(user);

            var response = await _aiChatService.GenerateTextToImagePromptAsync(command.Prompt, command.Image, cancellationToken);
            Validator.ValidateNotNull(response);

            return Result.Ok(response);
        }
    }
}