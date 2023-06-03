using FluentValidation;

namespace Beacon.Common.Validation.Rules;

public static class IsValidLaboratoryNameRule
{
    public static IRuleBuilderOptions<T, string> IsValidLaboratoryName<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty().WithMessage("Laboratory name is required.")
            .MaximumLength(50).WithMessage("Laboratory name cannot exceed 50 characters.");
    }
}
