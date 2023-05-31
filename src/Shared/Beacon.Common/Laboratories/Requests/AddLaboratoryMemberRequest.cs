using ErrorOr;
using FluentValidation;

namespace Beacon.Common.Laboratories.Requests;

public class AddLaboratoryMemberRequest : IApiRequest<Success>
{
    public Guid LaboratoryId { get; set; }
    public Guid NewMemberUserId { get; set; }

    public class Validator : AbstractValidator<AddLaboratoryMemberRequest>
    {
        public Validator()
        {
            RuleFor(x => x.LaboratoryId)
                .NotEmpty().WithMessage("Laboratory must be specified.");

            RuleFor(x => x.NewMemberUserId)
                .NotEmpty().WithMessage("New member must be specified.");
        }
    }
}
