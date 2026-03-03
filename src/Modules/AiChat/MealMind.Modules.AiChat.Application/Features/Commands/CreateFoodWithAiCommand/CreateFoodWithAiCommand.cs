using MealMind.Modules.AiChat.Application.Abstractions;
using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Dto.Nutrition;
using MealMind.Shared.Contracts.Result;
using MealMind.Shared.Events.AiChat;
using Microsoft.AspNetCore.Http;

namespace MealMind.Modules.AiChat.Application.Features.Commands.CreateFoodWithAiCommand;

public record CreateFoodWithAiCommand(string FoodPrompt, IFormFile? ImageFile) : ICommand<bool>
{
    public sealed class Handler : ICommandHandler<CreateFoodWithAiCommand, bool>
    {
        private readonly IAiChatUserRepository _userRepository;
        private readonly IAiChatService _aiChatService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutboxService _outboxService;

        public Handler(IAiChatUserRepository userRepository, IAiChatService aiChatService, IUserService userService, IUnitOfWork unitOfWork,
            IOutboxService outboxService)
        {
            _userRepository = userRepository;
            _aiChatService = aiChatService;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _outboxService = outboxService;
        }

        public async Task<Result<bool>> Handle(CreateFoodWithAiCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUserIdAsync(_userService.UserId, cancellationToken);
            if (user is null)
                return Result<bool>.NotFound("AI Chat user not found.");

            var response = await _aiChatService.CreateFoodFromPromptAsync(command.FoodPrompt, command.ImageFile ?? null, cancellationToken);

            await _outboxService.SaveAsync(
                new FoodCreatedEvent(
                    new FoodDto
                    {
                        Name = response.Name,
                        Brand = response.Brand,
                        NutritionPer100G = new NutrimentsPer100GDto
                        {
                            Calories = response.NutritionPer100G.Calories,
                            Protein = response.NutritionPer100G.Protein,
                            Carbohydrates = response.NutritionPer100G.Carbohydrates,
                            Fat = response.NutritionPer100G.Fat,
                            Fiber = response.NutritionPer100G.Fiber,
                            Sugar = response.NutritionPer100G.Sugar,
                            SaturatedFat = response.NutritionPer100G.SaturatedFat,
                            Sodium = response.NutritionPer100G.Sodium,
                            Salt = response.NutritionPer100G.Salt,
                            Cholesterol = response.NutritionPer100G.Cholesterol
                        },
                        CreatedAt = DateTime.UtcNow,
                        FoodSource = response.FoodSource
                    }
                ), cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<bool>.Ok(true);
        }
    }
}