using MealMind.Modules.AiChat.Application.Abstractions;
using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Modules.AiChat.Application.Dtos;
using MealMind.Modules.AiChat.Domain.ImageAnalyze;
using MealMind.Shared.Abstractions.Events.Integration;
using MealMind.Shared.Abstractions.Kernel.CommandValidators;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;
using Microsoft.AspNetCore.Http;

namespace MealMind.Modules.AiChat.Application.Features.Commands.GetCaloriesFromImageCommand;

public record GetCaloriesFromImageCommand(
    string? Prompt,
    EstimationMode Mode,
    IFormFile Image,
    DateOnly DailyLogDate,
    bool SaveFoodEntry = true
)
    : ICommand<AnalyzedImageStructuredResponse>
{
    public sealed class Handler : ICommandHandler<GetCaloriesFromImageCommand, AnalyzedImageStructuredResponse>
    {
        private readonly IAiChatUserRepository _userRepository;
        private readonly IImageAnalyzeRepository _imageAnalyzeRepository;
        private readonly IAiChatService _aiChatService;
        private readonly IUserService _userService;
        private readonly IPublisher _publisher;
        private readonly IUnitOfWork _unitOfWork;


        public Handler(IAiChatUserRepository userRepository, IImageAnalyzeRepository imageAnalyzeRepository, IAiChatService aiChatService,
            IUserService userService, IPublisher publisher, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _imageAnalyzeRepository = imageAnalyzeRepository;
            _aiChatService = aiChatService;
            _userService = userService;
            _publisher = publisher;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AnalyzedImageStructuredResponse>> Handle(GetCaloriesFromImageCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUserIdAsync(_userService.UserId, cancellationToken);
            Validator.ValidateNotNull(user);

            var response = await _aiChatService.GenerateTextToImagePromptAsync(command.Prompt, command.Image, cancellationToken);
            Validator.ValidateNotNull(response);

            var foodImageAnalyze = ImageAnalyze.Create(
                user.Id, response.FoodName, command.Prompt, null, response.ImageBytes,
                response.TotalMinEstimatedCalories, response.TotalMaxEstimatedCalories,
                response.TotalMinEstimatedProteins, response.TotalMaxEstimatedProteins,
                response.TotalMinEstimatedCarbohydrates, response.TotalMaxEstimatedCarbohydrates,
                response.TotalMinEstimatedFats, response.TotalMaxEstimatedFats,
                response.TotalConfidenceScore, response.TotalQuantityInGrams,
                command.SaveFoodEntry ? DateTime.UtcNow : null
            );

            await _imageAnalyzeRepository.AddAsync(foodImageAnalyze, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            if (foodImageAnalyze.SavedAt == null)
                return Result.Ok(response);

            var caloriesEstimation = CalculateEstimation(
                command.Mode,
                response.TotalMinEstimatedCalories,
                response.TotalMaxEstimatedCalories);

            var proteinsEstimation = CalculateEstimation(
                command.Mode,
                response.TotalMinEstimatedProteins,
                response.TotalMaxEstimatedProteins);

            var carbohydratesEstimation = CalculateEstimation(
                command.Mode,
                response.TotalMinEstimatedCarbohydrates,
                response.TotalMaxEstimatedCarbohydrates);

            var fatsEstimation = CalculateEstimation(
                command.Mode,
                response.TotalMinEstimatedFats,
                response.TotalMaxEstimatedFats);

            await _publisher.Publish(
                new ImageAnalyzeCreatedEvent(
                    user.Id,
                    foodImageAnalyze.FoodName,
                    foodImageAnalyze.TotalQuantityInGrams,
                    caloriesEstimation, proteinsEstimation,
                    carbohydratesEstimation, fatsEstimation,
                    command.DailyLogDate),
                cancellationToken);

            return Result.Ok(response);
        }
    }

    private static decimal CalculateEstimation(EstimationMode nutritionEstimationMode, decimal min, decimal max)
    {
        return nutritionEstimationMode switch
        {
            EstimationMode.Minimal => min,
            EstimationMode.Average => (min + max) / 2,
            EstimationMode.Maximal => max,
            _ => throw new ApplicationException("Invalid estimation mode.")
        };
    }
}