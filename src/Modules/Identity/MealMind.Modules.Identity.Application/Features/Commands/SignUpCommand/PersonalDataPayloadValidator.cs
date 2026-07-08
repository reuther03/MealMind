using FluentValidation;
using MealMind.Shared.Contracts.Dto.Nutrition;

namespace MealMind.Modules.Identity.Application.Features.Commands.SignUpCommand;

public class PersonalDataPayloadValidator : AbstractValidator<PersonalDataPayload>
{
    public PersonalDataPayloadValidator()
    {
        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Gender must be a valid value.");

        RuleFor(x => x.ActivityLevel)
            .IsInEnum().WithMessage("Activity level must be a valid value.");

        RuleFor(x => x.DateOfBirth)
            .Must(BeReasonablePastDate).WithMessage("Date of birth must be a past date within the last 130 years.");

        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("Weight must be greater than 0.")
            .LessThanOrEqualTo(500).WithMessage("Weight seems unreasonably high.");

        RuleFor(x => x.Height)
            .GreaterThan(0).WithMessage("Height must be greater than 0.")
            .LessThanOrEqualTo(300).WithMessage("Height seems unreasonably high (cm).");

        RuleFor(x => x.WeightTarget)
            .GreaterThan(0).WithMessage("Weight target must be greater than 0.")
            .LessThanOrEqualTo(500).WithMessage("Weight target seems unreasonably high.");
    }

    private static bool BeReasonablePastDate(DateOnly date)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return date < today && date > today.AddYears(-130);
    }
}
