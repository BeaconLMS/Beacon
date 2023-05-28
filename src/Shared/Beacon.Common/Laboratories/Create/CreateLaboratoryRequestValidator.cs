using FluentValidation;

namespace Beacon.Common.Laboratories.Create;

public sealed class CreateLaboratoryRequestValidator : AbstractValidator<CreateLaboratoryRequest>
{
    public CreateLaboratoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Laboratory name is required.")
            .MinimumLength(3).WithMessage("Laboratory name must be at least three characters.")
            .MaximumLength(50).WithMessage("Laboratory name cannot exceed 50 characters.");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required.")
            .MinimumLength(3).WithMessage("Slug must be at least three characters.")
            .MaximumLength(20).WithMessage("Slug cannot exceed 20 characters.");
    }
}
