using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Contracts.Dto.Training;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.Training.Application.Features.Queries;

public class GetTrainingSessionDetailsQuery : IQuery<TrainingSessionDetailsDto>
{
    public sealed class Handler : IQueryHandler<GetTrainingSessionDetailsQuery, TrainingSessionDetailsDto>
    {
        public Task<Result<TrainingSessionDetailsDto>> Handle(GetTrainingSessionDetailsQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}