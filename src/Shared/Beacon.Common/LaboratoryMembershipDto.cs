namespace Beacon.Common;

public sealed record LaboratoryMembershipDto
{
    public required Guid LaboratoryId { get; init; }
    public required string LaboratoryName { get; init; }
    public required string LaboratorySlug { get; init; }
    public required string MembershipType { get; init; }
}