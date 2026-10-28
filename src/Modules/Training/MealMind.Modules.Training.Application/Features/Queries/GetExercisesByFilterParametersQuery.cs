using LinqKit;
using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Modules.Training.Application.Features.Caching;
using MealMind.Modules.Training.Application.Features.Mappers;
using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Shared.Abstractions.QueriesAndCommands.Extensions;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Abstractions.Services;
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
        private readonly ICacheService _cacheService;

        public Handler(ITrainingDbContext dbContext, ICacheService cacheService)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
        }

        public async Task<Result<PaginatedList<ExerciseDto>>> Handle(GetExercisesByFilterParametersQuery query,
            CancellationToken cancellationToken = default)
        {
            var exercises = new List<ExerciseDto>();

            if (query.IsCustom != true)
                exercises.AddRange(await GetBaseExercisesAsync(cancellationToken));

            if (query.IsCustom != false)
                exercises.AddRange(await GetCustomExercisesAsync(cancellationToken));

            var name = query.Name;
            var type = query.ExerciseType?.ToString();
            var muscleGroup = query.MuscleGroup?.ToString();

            var filtered = exercises
                .WhereIf(!string.IsNullOrWhiteSpace(name),
                    x => x.Name.Contains(name!, StringComparison.OrdinalIgnoreCase))
                .WhereIf(type is not null, x => x.Type == type)
                .WhereIf(muscleGroup is not null, x => x.MuscleGroup == muscleGroup)
                .OrderBy(x => x.Name)
                .ToList();

            var paginated = filtered
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            return Result<PaginatedList<ExerciseDto>>.Ok(new PaginatedList<ExerciseDto>(
                query.PageNumber,
                query.PageSize,
                filtered.Count,
                paginated));
        }

        private async Task<List<ExerciseDto>> GetBaseExercisesAsync(CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeyBuilder.GetAllBaseExercisesKey();

            var baseExercises = await _cacheService.GetAsync<List<ExerciseDto>>(cacheKey);
            if (baseExercises is not null)
                return baseExercises;

            baseExercises = await _dbContext.Exercises
                .AsExpandable()
                .Where(x => !x.IsCustom)
                .Select(x => ExerciseMapper.ExerciseProjection.Invoke(x))
                .ToListAsync(cancellationToken);

            await _cacheService.SetAsync(cacheKey, baseExercises);

            return baseExercises;
        }

        private Task<List<ExerciseDto>> GetCustomExercisesAsync(CancellationToken cancellationToken)
            => _dbContext.Exercises
                .AsExpandable()
                .Where(x => x.IsCustom)
                .Select(x => ExerciseMapper.ExerciseProjection.Invoke(x))
                .ToListAsync(cancellationToken);
    }
}
