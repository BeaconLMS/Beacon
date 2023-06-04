namespace Beacon.Common.Laboratories;

public record LaboratoryMembershipDto
{
    public required LaboratoryDto Laboratory { get; init; }
    public required string MembershipType { get; init; }
}
