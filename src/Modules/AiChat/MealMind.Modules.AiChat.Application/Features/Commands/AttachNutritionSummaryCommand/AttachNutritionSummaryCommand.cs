using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.AiChat.Application.Features.Commands.AttachNutritionSummaryCommand;

public class AttachNutritionSummaryCommand : ICommand<bool>
{
    public sealed class Handler : ICommandHandler<AttachNutritionSummaryCommand, bool>
    {
        private readonly INutritionSummaryService _nutritionSummaryService;

        public Task<Result<bool>> Handle(AttachNutritionSummaryCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}