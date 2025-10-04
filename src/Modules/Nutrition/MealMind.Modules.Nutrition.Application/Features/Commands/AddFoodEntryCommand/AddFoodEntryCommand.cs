using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Shared.Abstractions.Kernel.Primitives.Result;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;

namespace MealMind.Modules.Nutrition.Application.Features.Commands.AddFoodEntryCommand;

public record AddFoodEntryCommand(MealPayload? MealPayload, FoodId FoodId) : ICommand<Guid>
{
    internal sealed class Handler : ICommandHandler<AddFoodEntryCommand, Guid>
    {
        private readonly IFoodRepository _foodRepository;

        public Handler(IFoodRepository foodRepository)
        {
            _foodRepository = foodRepository;
        }

        public Task<Result<Guid>> Handle(AddFoodEntryCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}