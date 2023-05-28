﻿namespace Beacon.API.Entities;

public class Laboratory
{
    public required Guid Id { get; init; }
    public required string Name { get; set; }
    public required string Slug { get; set; }

    public List<LaboratoryMembership> Memberships { get; set; } = new();

    public LaboratoryMembership AddMember(User member, LabMembershipType membershipType)
    {
        var membership = new LaboratoryMembership
        {
            Id = Guid.NewGuid(),
            Laboratory = this,
            Member = member,
            MembershipType = membershipType
        };

        Memberships.Add(membership);

        return membership;
    }
}
