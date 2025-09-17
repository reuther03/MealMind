using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;

namespace MealMind.Shared.Abstractions.Kernel.Payloads;

public record PersonalDataPayload(
    Gender Gender,
    DateOnly DateOfBirth,
    decimal Weight,
    decimal Height,
    decimal WeightTarget,
    ActivityLevel ActivityLevel);