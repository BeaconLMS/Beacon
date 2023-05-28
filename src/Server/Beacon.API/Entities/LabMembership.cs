namespace Beacon.API.Entities;

public sealed class LabMembership
{
    public required Guid Id { get; init; }
    public required LabMembershipType MembershipType { get; init; }

    public required Laboratory Laboratory { get; set; }
    public required User User { get; set; }
}
