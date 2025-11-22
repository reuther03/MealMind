using MealMind.Modules.AiChat.Application.Abstractions;
using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Modules.AiChat.Application.Dtos;
using MealMind.Modules.AiChat.Domain.ImageAnalyze;
using MealMind.Shared.Abstractions.Extensions;
using MealMind.Shared.Abstractions.Kernel.CommandValidators;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;
using Microsoft.AspNetCore.Http;

namespace MealMind.Modules.AiChat.Application.Features.Commands.GetCaloriesFromImageCommand;

public record GetCaloriesFromImageCommand(string? Prompt, IFormFile Image) : ICommand<AnalyzedImageStructuredResponse>
{
    public sealed class Handler : ICommandHandler<GetCaloriesFromImageCommand, AnalyzedImageStructuredResponse>
    {
        private readonly IAiChatUserRepository _userRepository;
        private readonly IImageAnalyzeRepository _imageAnalyzeRepository;
        private readonly IAiChatService _aiChatService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;


        public Handler(IAiChatUserRepository userRepository, IImageAnalyzeRepository imageAnalyzeRepository, IAiChatService aiChatService,
            IUserService userService, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _imageAnalyzeRepository = imageAnalyzeRepository;
            _aiChatService = aiChatService;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AnalyzedImageStructuredResponse>> Handle(GetCaloriesFromImageCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUserIdAsync(_userService.UserId, cancellationToken);
            Validator.ValidateNotNull(user);

            var response = await _aiChatService.GenerateTextToImagePromptAsync(command.Prompt, command.Image, cancellationToken);

            var foodImageAnalyze = ImageAnalyze.Create(user.Id, command.Prompt, null, response.ImageBytes, response.TotalMinEstimatedCalories,
                response.TotalMaxEstimatedCalories, response.TotalMaxEstimatedProteins, response.TotalMaxEstimatedProteins,
                response.TotalMinEstimatedCarbohydrates, response.TotalMaxEstimatedCarbohydrates, response.TotalMinEstimatedFats,
                response.TotalMaxEstimatedFats, response.TotalConfidenceScore);

            Validator.ValidateNotNull(response);

            await _imageAnalyzeRepository.AddAsync(foodImageAnalyze, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.Ok(response);
        }
    }
}