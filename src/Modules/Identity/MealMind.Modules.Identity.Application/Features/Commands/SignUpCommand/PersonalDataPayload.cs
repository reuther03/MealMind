using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;

namespace MealMind.Modules.Identity.Application.Features.Commands.SignUpCommand;

public record PersonalDataPayload(Gender Gender, DateOnly DateOfBirth, decimal Weight, decimal Height, decimal WeightTarget, ActivityLevel ActivityLevel);