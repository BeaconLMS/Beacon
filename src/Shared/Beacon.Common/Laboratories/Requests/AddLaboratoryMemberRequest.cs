using ErrorOr;
using FluentValidation;

namespace Beacon.Common.Laboratories.Requests;

public class AddLaboratoryMemberRequest : IApiRequest<Success>
{
    public Guid LaboratoryId { get; set; }
    public Guid UserId { get; set; }

    public class Validator : AbstractValidator<AddLaboratoryMemberRequest>
    {
        public Validator()
        {
            RuleFor(x => x.LaboratoryId)
                .NotEmpty().WithMessage("LaboratoryId is required.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");
        }
    }
}
