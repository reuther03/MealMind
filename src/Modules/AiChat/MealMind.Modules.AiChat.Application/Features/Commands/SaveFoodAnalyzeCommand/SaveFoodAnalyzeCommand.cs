using MealMind.Modules.AiChat.Application.Abstractions;
using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Application.Features.Commands.GetCaloriesFromImageCommand;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;
using MealMind.Shared.Events.AiChat;

namespace MealMind.Modules.AiChat.Application.Features.Commands.SaveFoodAnalyzeCommand;

public record SaveFoodAnalyzeCommand(
    ImageAnalyzeSessionId SessionId,
    Guid? CorrectionId,
    EstimationMode EstimationMode,
    DateOnly LogDate
) : ICommand<bool>
{
    public sealed class Handler : ICommandHandler<SaveFoodAnalyzeCommand, bool>
    {
        private readonly IAiChatUserRepository _userRepository;
        private readonly IImageAnalyzeRepository _imageAnalyzeRepository;
        private readonly IUserService _userService;
        private readonly IOutboxService _outboxService;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IAiChatUserRepository userRepository, IImageAnalyzeRepository imageAnalyzeRepository, IUserService userService,
            IOutboxService outboxService, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _imageAnalyzeRepository = imageAnalyzeRepository;
            _userService = userService;
            _outboxService = outboxService;
            _unitOfWork = unitOfWork;
        }


        public async Task<Result<bool>> Handle(SaveFoodAnalyzeCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUserIdAsync(_userService.UserId, cancellationToken);
            if (user is null)
                return Result<bool>.NotFound("AI Chat user not found.");

            var session = await _imageAnalyzeRepository.GetByIdAsync(command.SessionId, user.Id, cancellationToken);
            if (session is null)
                return Result<bool>.NotFound("Image analyze session not found.");

            var imageAnalyze = command.CorrectionId == null
                ? session.ImageAnalyze
                : session.Corrections.FirstOrDefault(c => c.Id == command.CorrectionId);

            if (imageAnalyze is null)
                return Result<bool>.NotFound("Image analyze correction not found.");

            var caloriesEstimation = CalculateEstimation(
                command.EstimationMode,
                imageAnalyze.CaloriesMin,
                imageAnalyze.CaloriesMax);

            var proteinsEstimation = CalculateEstimation(
                command.EstimationMode,
                imageAnalyze.ProteinMin,
                imageAnalyze.ProteinMax);

            var carbohydratesEstimation = CalculateEstimation(
                command.EstimationMode,
                imageAnalyze.CaloriesMin,
                imageAnalyze.CarbsMax);

            var fatsEstimation = CalculateEstimation(
                command.EstimationMode,
                imageAnalyze.FatMin,
                imageAnalyze.FatMax);

            await _outboxService.SaveAsync(
                new ImageAnalyzeCreatedEvent(
                    user.Id,
                    imageAnalyze.FoodName,
                    imageAnalyze.TotalQuantityInGrams,
                    caloriesEstimation, proteinsEstimation,
                    carbohydratesEstimation, fatsEstimation,
                    command.LogDate),
                cancellationToken);

            return Result.Ok(true);
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