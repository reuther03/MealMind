using MealMind.Shared.Abstractions.Kernel.Payloads;
using MealMind.Shared.Abstractions.Kernel.Primitives.Result;

namespace MealMind.Shared.Abstractions.Services;

public interface IAuthService
{
    Task<Result<ExternalUserPayload>> GetTokenClaimsAsync(string token);
}