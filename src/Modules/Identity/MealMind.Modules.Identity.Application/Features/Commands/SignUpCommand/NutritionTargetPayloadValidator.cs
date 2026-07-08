using FluentValidation;
using MealMind.Shared.Contracts.Dto.Nutrition;

namespace MealMind.Modules.Identity.Application.Features.Commands.SignUpCommand;

public class NutritionTargetPayloadValidator : AbstractValidator<NutritionTargetPayload>
{
    public NutritionTargetPayloadValidator()
    {
        RuleFor(x => x.Calories)
            .GreaterThan(0).WithMessage("Calories must be greater than 0.")
            .LessThanOrEqualTo(10000).WithMessage("Calories seem unreasonably high.");

        RuleFor(x => x.WaterIntake)
            .GreaterThanOrEqualTo(0).WithMessage("Water intake cannot be negative.")
            .LessThanOrEqualTo(20).WithMessage("Water intake seems unreasonably high (L).");

        RuleFor(x => x.ActiveDays)
            .NotEmpty().WithMessage("At least one active day is required.");

        RuleFor(x => x.ActiveDays!)
            .Must(days => days.Distinct().Count() == days.Count)
            .WithMessage("Active days must be unique.")
            .When(x => x.ActiveDays is not null);

        RuleForEach(x => x.ActiveDays)
            .IsInEnum().WithMessage("Active day must be a valid day of the week.");

        RuleFor(x => x)
            .Must(x => x.NutritionInGramsPayload is not null || x.NutritionInPercentPayload is not null)
            .WithMessage("Either grams or percent nutrition breakdown must be provided.");

        When(x => x.NutritionInGramsPayload is not null, () =>
        {
            RuleFor(x => x.NutritionInGramsPayload!.ProteinInGrams)
                .GreaterThanOrEqualTo(0).WithMessage("Protein (g) cannot be negative.");
            RuleFor(x => x.NutritionInGramsPayload!.CarbohydratesInGrams)
                .GreaterThanOrEqualTo(0).WithMessage("Carbohydrates (g) cannot be negative.");
            RuleFor(x => x.NutritionInGramsPayload!.FatsInGrams)
                .GreaterThanOrEqualTo(0).WithMessage("Fats (g) cannot be negative.");
        });

        When(x => x.NutritionInPercentPayload is not null, () =>
        {
            RuleFor(x => x.NutritionInPercentPayload!.ProteinInPercent)
                .InclusiveBetween(0, 100).WithMessage("Protein (%) must be between 0 and 100.");
            RuleFor(x => x.NutritionInPercentPayload!.CarbohydratesInPercent)
                .InclusiveBetween(0, 100).WithMessage("Carbohydrates (%) must be between 0 and 100.");
            RuleFor(x => x.NutritionInPercentPayload!.FatsInPercent)
                .InclusiveBetween(0, 100).WithMessage("Fats (%) must be between 0 and 100.");

            RuleFor(x => x.NutritionInPercentPayload!)
                .Must(p => Math.Abs(p.ProteinInPercent + p.CarbohydratesInPercent + p.FatsInPercent - 100) < 0.01m)
                .WithMessage("Percent macros must sum to 100.");
        });
    }
}
