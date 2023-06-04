﻿namespace Beacon.API.Domain.Entities;

public class LaboratoryInvitation
{
    public required Guid Id { get; init; }
    public required DateTimeOffset CreatedOn { get; init; }
    public required TimeSpan ExpirationTimeSpan { get; init; }

    public required string NewMemberEmailAddress { get; init; }
    public required LaboratoryMembershipType MembershipType { get; init; }

    public required Guid LaboratoryId { get; init; }
    public Laboratory Laboratory { get; init; } = null!;

    public required Guid CreatedById { get; init; }
    public User CreatedBy { get; init; } = null!;

    private readonly List<LaboratoryInvitationEmail> _emailInvitations = new();
    public IReadOnlyList<LaboratoryInvitationEmail> EmailInvitations => _emailInvitations.AsReadOnly();

    public LaboratoryInvitationEmail AddEmailInvitation()
    {
        var invitationEmail = new LaboratoryInvitationEmail
        {
            Id = Guid.NewGuid(),
            LaboratoryInvitationId = Id
        };

        _emailInvitations.Add(invitationEmail);

        return invitationEmail;
    }
}
