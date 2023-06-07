using Beacon.Common.Laboratories.Enums;
using FluentValidation;

namespace Beacon.Common.Laboratories.Requests;

public class UpdateMembershipTypeRequest
{
    public Guid MemberId { get; set; }
    public LaboratoryMembershipType MembershipType { get; set; } = LaboratoryMembershipType.Member;

    public sealed class Validator : AbstractValidator<UpdateMembershipTypeRequest>
    {
        public Validator()
        {
            RuleFor(x => x.MemberId)
                .NotEmpty().WithMessage("MemberId is required.");
        }
    }
}
