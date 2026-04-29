using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Shared.Abstractions.Messaging.AiChat;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Dto.Nutrition;
using MealMind.Shared.Contracts.Result;
using Microsoft.Extensions.Logging;

namespace MealMind.Modules.Nutrition.Application.Features.Commands.AddCustomFoodCommand;

public record AddCustomFoodCommand(
    string Name,
    string? Barcode,
    NutritionPer100G NutritionPer100G,
    string ImageUrl,
    string Brand,
    List<Category>? Categories,
    List<DietaryTag>? DietaryTags
) : ICommand<Guid>
{
    public sealed class Handler : ICommandHandler<AddCustomFoodCommand, Guid>
    {
        private readonly IFoodRepository _foodRepository;
        private readonly IUserService _userService;
        private readonly IUserProfileRepository _profileRepository;
        private readonly ILogger<AddCustomFoodCommand> _logger;
        private readonly ISender _sender;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IFoodRepository foodRepository, IUserService userService, IUserProfileRepository profileRepository, ILogger<AddCustomFoodCommand> logger,
            ISender sender,
            IUnitOfWork unitOfWork)
        {
            _foodRepository = foodRepository;
            _userService = userService;
            _profileRepository = profileRepository;
            _logger = logger;
            _sender = sender;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(AddCustomFoodCommand command, CancellationToken cancellationToken)
        {
            var categories = command.Categories?.ToList() ?? [];
            var dietaryTags = command.DietaryTags?.ToList() ?? [];

            var user = await _profileRepository.GetByIdAsync(_userService.UserId!, cancellationToken);
            if (user is null)
                return Result<Guid>.BadRequest("User profile not found.");

            // TODO: Refactor to background service if scaling is needed
            if (categories.Count == 0 && dietaryTags.Count == 0)
            {
                try
                {
                    var foodTagsResult = await _sender
                        .Send(new GenerateFoodTagsQuery(command.Name, command.Brand), cancellationToken);

                    if (foodTagsResult is { IsSuccess: true, Value: not null })
                    {
                        categories.AddRange(
                            foodTagsResult.Value.Categories
                                .Select(s => Enum.TryParse<Category>(s, ignoreCase: true, out var c) ? (Category?)c : null)
                                .Where(c => c is not null)
                                .Select(c => c!.Value));

                        dietaryTags.AddRange(
                            foodTagsResult.Value.DietaryTags
                                .Select(s => Enum.TryParse<DietaryTag>(s, ignoreCase: true, out var t) ? (DietaryTag?)t : null)
                                .Where(t => t is not null)
                                .Select(t => t!.Value));

                        if (categories.Count == 0 && dietaryTags.Count == 0)
                        {
                            return Result<Guid>.BadRequest(
                                "Could not infer tags for this food. Please provide at least one category or dietary tag.");
                        }
                    }
                    else
                    {
                        return Result<Guid>.BadRequest(
                            "Automatic tagging is currently unavailable. Please provide at least one category or dietary tag.");
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "AI tag generation failed for {FoodName}.", command.Name);
                    return Result<Guid>.BadRequest(
                        "Automatic tagging is currently unavailable. Please provide at least one category or dietary tag.");
                }
            }

            var food = Food.Create(
                command.Name, command.NutritionPer100G,
                FoodDataSource.User, command.Barcode,
                command.ImageUrl, command.Brand);

            food.AssignTags(categories, dietaryTags);

            await _foodRepository.AddAsync(food, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<Guid>.Ok(food.Id.Value);
        }
    }
}