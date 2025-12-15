using MealMind.Modules.AiChat.Application.Abstractions;
using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Modules.AiChat.Application.Dtos;
using MealMind.Modules.AiChat.Domain.ImageAnalyze;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;
using Microsoft.AspNetCore.Http;

namespace MealMind.Modules.AiChat.Application.Features.Commands.GetCaloriesFromImageCommand;

public record GetCaloriesFromImageCommand(
    Guid? SessionId,
    string? Prompt,
    List<UserProvidedFoodProductsPayload>? UserProvidedFoods,
    IFormFile Image
)
    : ICommand<AnalyzedImageStructuredResponse>
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
            if (user is null)
                return Result<AnalyzedImageStructuredResponse>.NotFound("AI Chat user not found.");

            var response = await _aiChatService.AnalyzeImageWithPromptAsync(command.Prompt, command.UserProvidedFoods ?? [], command.Image, cancellationToken);

            var foodImageAnalyze = ImageAnalyze.Create(
                Guid.Empty, response.FoodName, command.Prompt, null, response.ImageBytes,
                response.TotalMinEstimatedCalories, response.TotalMaxEstimatedCalories,
                response.TotalMinEstimatedProteins, response.TotalMaxEstimatedProteins,
                response.TotalMinEstimatedCarbohydrates, response.TotalMaxEstimatedCarbohydrates,
                response.TotalMinEstimatedFats, response.TotalMaxEstimatedFats,
                response.TotalConfidenceScore, response.TotalQuantityInGrams
            );

            if (command.SessionId != null)
            {
                var existingSession = await _imageAnalyzeRepository.GetByIdAsync(command.SessionId.Value, user.Id, cancellationToken);
                if (existingSession is null)
                    return Result<AnalyzedImageStructuredResponse>.NotFound("Image analyze session not found.");

                existingSession.AddCorrection(foodImageAnalyze);
            }
            else
            {
                var session = ImageAnalyzeSession.Create(user.Id, foodImageAnalyze);
                await _imageAnalyzeRepository.AddAsync(session, cancellationToken);
            }

            await _unitOfWork.CommitAsync(cancellationToken);
            return Result.Ok(response);
        }
    }
}