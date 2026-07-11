using MealMind.Modules.Identity.Application.Abstractions.Database;
using MealMind.Modules.Identity.Application.Features.Mappers;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Dto.Identity;
using MealMind.Shared.Contracts.Result;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Identity.Application.Features.Queries;

public record GetCurrentUserQuery : IQuery<IdentityDto>
{
    public sealed class Handler : IQueryHandler<GetCurrentUserQuery, IdentityDto>
    {
        private readonly IUserService _userService;
        private readonly IIdentityDbContext _dbContext;

        public Handler(IUserService userService, IIdentityDbContext dbContext)
        {
            _userService = userService;
            _dbContext = dbContext;
        }

        public async Task<Result<IdentityDto>> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken)
        {
            var user = await _dbContext.IdentityUsers
                .Where(x => x.Id == _userService.UserId)
                .Select(IdentityUserMapper.Projection)
                .FirstOrDefaultAsync(cancellationToken);

            return user is null ? Result<IdentityDto>.NotFound("User not found.") : Result<IdentityDto>.Ok(user);
        }
    }
}
