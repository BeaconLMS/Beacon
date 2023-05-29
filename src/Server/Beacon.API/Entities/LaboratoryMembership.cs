namespace Beacon.API.Entities;

public class LaboratoryMembership
{
    public required Guid Id { get; init; }
    public required LaboratoryMembershipType MembershipType { get; init; }

    public required Laboratory Laboratory { get; set; }
    public required User Member { get; set; }
}
