using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Dto.Training;
using MealMind.Shared.Contracts.Pagination;
using MealMind.Shared.Contracts.Result;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Training.Application.Features.Queries;

public record GetAllTrainingPlansQuery(int Page, int PageSize) : IQuery<PaginatedList<TrainingPlanDto>>
{
    public sealed class Handler : IQueryHandler<GetAllTrainingPlansQuery, PaginatedList<TrainingPlanDto>>
    {
        private readonly ITrainingDbContext _dbContext;
        private readonly IUserService _userService;

        public Handler(ITrainingDbContext dbContext, IUserService userService)
        {
            _dbContext = dbContext;
            _userService = userService;
        }

        public async Task<Result<PaginatedList<TrainingPlanDto>>> Handle(GetAllTrainingPlansQuery query, CancellationToken cancellationToken = default)
        {
            var userId = _userService.UserId;

            var baseQuery = _dbContext.TrainingPlans.Where(x => x.UserId == userId);

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            var trainingPlansDto = await baseQuery
                .OrderByDescending(x => x.Sessions.Where(s => s.EndedAt != null).Max(s => s.EndedAt))
                .ThenBy(x => x.Name)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(x => new TrainingPlanDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    PlannedOn = x.PlannedOn,
                    IsActive = x.IsActive,
                    SessionsCount = x.Sessions.Count,
                    LastCompletedSessionAt = x.Sessions
                        .Where(s => s.EndedAt != null)
                        .Max(s => s.EndedAt)
                })
                .ToListAsync(cancellationToken);

            return Result<PaginatedList<TrainingPlanDto>>.Ok(new PaginatedList<TrainingPlanDto>(
                query.Page,
                query.PageSize,
                totalCount,
                trainingPlansDto));
        }
    }
}