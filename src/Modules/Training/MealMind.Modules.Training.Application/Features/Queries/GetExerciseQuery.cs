using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Contracts.Dto.Training;
using MealMind.Shared.Contracts.Result;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Training.Application.Features.Queries;

public record GetExerciseQuery(Guid ExerciseId) : IQuery<ExerciseDto>
{
    public sealed class Handler : IQueryHandler<GetExerciseQuery, ExerciseDto>
    {
        private readonly ITrainingDbContext _dbContext;

        public Handler(ITrainingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<ExerciseDto>> Handle(GetExerciseQuery query, CancellationToken cancellationToken = default)
        {
            var exercise = await _dbContext.Exercises.FirstOrDefaultAsync(x => x.Id == query.ExerciseId, cancellationToken);
            if (exercise == null)
                return Result<ExerciseDto>.BadRequest("Exercise not found.");

            var exerciseDto = new ExerciseDto
            {
                Id = exercise.Id,
                Name = exercise.Name,
                Description = exercise.Description,
                ImageUrl = exercise.ImageUrl,
                VideoUrl = exercise.VideoUrl,
                Type = exercise.Type.ToString(),
                MuscleGroup = exercise.MuscleGroup?.ToString(),
                IsCustom = exercise.IsCustom
            };

            return Result<ExerciseDto>.Ok(exerciseDto);
        }
    }
}