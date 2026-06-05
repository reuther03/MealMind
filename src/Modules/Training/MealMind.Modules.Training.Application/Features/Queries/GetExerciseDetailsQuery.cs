using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Contracts.Dto.Training;
using MealMind.Shared.Contracts.Result;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Training.Application.Features.Queries;

public record GetExerciseDetailsQuery(Guid ExerciseId) : IQuery<ExerciseDetailsDto>
{
    public sealed class Handler : IQueryHandler<GetExerciseDetailsQuery, ExerciseDetailsDto>
    {
        private readonly ITrainingDbContext _dbContext;

        public Handler(ITrainingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<ExerciseDetailsDto>> Handle(GetExerciseDetailsQuery detailsQuery, CancellationToken cancellationToken = default)
        {
            var exercise = await _dbContext.Exercises.FirstOrDefaultAsync(x => x.Id == detailsQuery.ExerciseId, cancellationToken);
            if (exercise == null)
                return Result<ExerciseDetailsDto>.BadRequest("Exercise not found.");

            var exerciseDto = new ExerciseDetailsDto
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

            return Result<ExerciseDetailsDto>.Ok(exerciseDto);
        }
    }
}