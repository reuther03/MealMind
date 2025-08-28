using System.Diagnostics.CodeAnalysis;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using UserId = MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids.UserId;

namespace MealMind.Shared.Abstractions.Services;

public interface IUserService
{
    [MemberNotNullWhen(true, nameof(UserId), nameof(Email), nameof(UserName))]
    public bool IsAuthenticated { get; }

    public UserId? UserId { get; }
    public Email? Email { get; }
    public Name? UserName { get; }
}