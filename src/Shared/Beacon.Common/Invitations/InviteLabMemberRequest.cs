﻿using Beacon.Common.Memberships;
using FluentValidation;

namespace Beacon.Common.Invitations;

public sealed class InviteLabMemberRequest
{
    public string NewMemberEmailAddress { get; set; } = string.Empty;
    public LaboratoryMembershipType MembershipType { get; set; } = LaboratoryMembershipType.Member;

    public sealed class Validator : AbstractValidator<InviteLabMemberRequest>
    {
        public Validator()
        {
            RuleFor(x => x.NewMemberEmailAddress)
                .EmailAddress().WithMessage("Invalid email address.");
        }
    }
}