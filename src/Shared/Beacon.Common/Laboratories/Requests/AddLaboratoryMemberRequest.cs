using ErrorOr;
using FluentValidation;

namespace Beacon.Common.Laboratories.Requests;

public class AddLaboratoryMemberRequest : IApiRequest<Success>
{
    public Guid LaboratoryId { get; set; }
    public string NewMemberEmailAddress { get; set; } = string.Empty;

    public class Validator : AbstractValidator<AddLaboratoryMemberRequest>
    {
        public Validator()
        {
            RuleFor(x => x.LaboratoryId)
                .NotEmpty().WithMessage("Laboratory must be specified.");

            RuleFor(x => x.NewMemberEmailAddress)
                .EmailAddress().WithMessage("Valid email address is required.");
        }
    }
}
