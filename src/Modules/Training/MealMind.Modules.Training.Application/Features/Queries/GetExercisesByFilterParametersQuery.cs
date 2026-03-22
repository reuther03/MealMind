using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Shared.Abstractions.QueriesAndCommands.Extensions;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Contracts.Dto.Training;
using MealMind.Shared.Contracts.Pagination;
using MealMind.Shared.Contracts.Result;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Training.Application.Features.Queries;

public record GetExercisesByFilterParametersQuery(
    string? Name,
    ExerciseType? ExerciseType,
    MuscleGroup? MuscleGroup,
    bool? IsCustom,
    int PageNumber = 1,
    int PageSize = 10)
    : IQuery<PaginatedList<ExerciseDto>>
{
    public sealed class Handler : IQueryHandler<GetExercisesByFilterParametersQuery, PaginatedList<ExerciseDto>>
    {
        private readonly ITrainingDbContext _dbContext;

        public Handler(ITrainingDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<Result<PaginatedList<ExerciseDto>>> Handle(GetExercisesByFilterParametersQuery query, CancellationToken cancellationToken = default)
        {
            var exercises = _dbContext.Exercises
                .WhereIf(!string.IsNullOrEmpty(query.Name), x => EF.Functions.Like(x.Name, $"%{query.Name}%"))
                .WhereIf(query.ExerciseType.HasValue, x => x.Type == query.ExerciseType)
                .WhereIf(query.MuscleGroup.HasValue, x => x.MuscleGroup == query.MuscleGroup)
                .WhereIf(query.IsCustom.HasValue, x => x.IsCustom == query.IsCustom);

            var totalCount = await exercises.CountAsync(cancellationToken);

            var exercisesPaginated = exercises
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(x => new ExerciseDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    ImageUrl = x.ImageUrl,
                    Type = x.Type.ToString(),
                    MuscleGroup = x.MuscleGroup.ToString(),
                    IsCustom = x.IsCustom
                })
                .ToListAsync(cancellationToken);

            return Result<PaginatedList<ExerciseDto>>.Ok(new PaginatedList<ExerciseDto>(query.PageNumber, query.PageSize, totalCount, exercisesPaginated));
        }
    }
}