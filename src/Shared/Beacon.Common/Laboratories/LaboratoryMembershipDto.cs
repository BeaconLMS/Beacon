namespace Beacon.Common.Laboratories;

public record LaboratoryMembershipDto
{
    public required Guid LaboratoryId { get; init; }
    public required string LaboratoryName { get; init; }
    public required string MembershipType { get; init; }
}
