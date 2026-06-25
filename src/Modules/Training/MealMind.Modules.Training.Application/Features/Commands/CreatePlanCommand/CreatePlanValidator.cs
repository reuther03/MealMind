using FluentValidation;

namespace MealMind.Modules.Training.Application.Features.Commands.CreatePlanCommand;

public class CreatePlanValidator : AbstractValidator<CreatePlanCommand>
{
    public CreatePlanValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.PlannedAt)
            .IsInEnum().WithMessage("PlannedAt must be a valid day of the week.");
    }
}