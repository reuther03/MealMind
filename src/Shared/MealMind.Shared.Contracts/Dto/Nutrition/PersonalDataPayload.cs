using MealMind.Shared.Contracts.Types;

namespace MealMind.Shared.Contracts.Dto.Nutrition;

public record PersonalDataPayload(
    Gender Gender,
    DateOnly DateOfBirth,
    decimal Weight,
    decimal Height,
    decimal WeightTarget,
    ActivityLevel ActivityLevel);