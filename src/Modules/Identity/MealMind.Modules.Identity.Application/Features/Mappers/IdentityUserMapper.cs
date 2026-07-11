using System.Linq.Expressions;
using MealMind.Modules.Identity.Domain.IdentityUser;
using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Contracts.Dto.Identity;

namespace MealMind.Modules.Identity.Application.Features.Mappers;

public static class IdentityUserMapper
{
    public static Expression<Func<IdentityUser, IdentityDto>> Projection { get; } =
        entity => new IdentityDto
        {
            Id = entity.Id,
            Username = entity.Username,
            Email = entity.Email,
            SubscriptionTier = entity.Subscription.Tier.ToString()
        };
}
