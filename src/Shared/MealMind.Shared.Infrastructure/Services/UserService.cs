using System.Security.Claims;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using UserId = MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids.UserId;

namespace MealMind.Shared.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    public UserId? UserId => IsAuthenticated ? GetUserIdFromClaims(_httpContextAccessor.HttpContext?.User) : null;
    public Email? Email => IsAuthenticated ? GetEmailFromClaims(_httpContextAccessor.HttpContext?.User) : null;
    public Name? UserName => IsAuthenticated ? GetUserNameFromClaims(_httpContextAccessor.HttpContext?.User) : null;

    private static UserId? GetUserIdFromClaims(ClaimsPrincipal? claims)
    {
        if (claims is null)
            return null;

        var userId = claims.FindFirst(ClaimTypes.Name)?.Value;
        return userId is null ? null : UserId.From(userId);
    }

    private static Email? GetEmailFromClaims(ClaimsPrincipal? claims)
    {
        if (claims is null)
            return null;

        var email = claims.FindFirst(ClaimTypes.Email)?.Value;
        return email is null ? null : new Email(email);
    }

    private static Name? GetUserNameFromClaims(ClaimsPrincipal? claims)
    {
        if (claims is null)
            return null;

        var userName = claims.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
        return userName is null ? null : new Name(userName);
    }
}