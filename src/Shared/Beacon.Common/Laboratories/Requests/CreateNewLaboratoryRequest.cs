using FluentValidation;

namespace Beacon.Common.Laboratories.Requests;

public class CreateNewLaboratoryRequest : IApiRequest<LaboratoryDto>
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public class Validator : AbstractValidator<CreateNewLaboratoryRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.");

            RuleFor(x => x.Slug)
                .NotEmpty().WithMessage("Slug is required.")
                .MaximumLength(20).WithMessage("Slug cannot exceed 20 characters.");
        }
    }
}
