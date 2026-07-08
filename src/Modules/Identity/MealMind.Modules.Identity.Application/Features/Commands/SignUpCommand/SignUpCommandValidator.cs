using FluentValidation;

namespace MealMind.Modules.Identity.Application.Features.Commands.SignUpCommand;

public class SignUpCommandValidator : AbstractValidator<SignUpCommand>
{
    public SignUpCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
            .MaximumLength(100).WithMessage("Username cannot exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(x => x.InputPassword)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .MaximumLength(100).WithMessage("Password cannot exceed 100 characters.");

        RuleFor(x => x.PersonalData)
            .NotNull().WithMessage("Personal data is required.")
            .SetValidator(new PersonalDataPayloadValidator()!);

        RuleFor(x => x.NutritionTargets)
            .NotEmpty().WithMessage("At least one nutrition target is required.");

        RuleForEach(x => x.NutritionTargets)
            .SetValidator(new NutritionTargetPayloadValidator());
    }
}
