using FluentValidation;

namespace Beacon.Common.Laboratories.Requests;

public sealed class InviteLabMemberRequest
{
    public string NewMemberEmailAddress { get; set; } = string.Empty;
    public string MembershipType { get; set; } = "Member";

    public sealed class Validator : AbstractValidator<InviteLabMemberRequest>
    {
        public Validator()
        {
            RuleFor(x => x.NewMemberEmailAddress)
                .EmailAddress().WithMessage("Invalid email address.");

            RuleFor(x => x.MembershipType)
                .NotEmpty().WithMessage("Membership type is required.");
        }
    }
}
