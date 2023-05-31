namespace Beacon.Common.Laboratories;

public record LaboratoryMembershipDto
{
    public required Guid MemberId { get; init; }
    public required string MemberDisplayName { get; init; }
    public required string MembershipType { get; init; }

    public required Guid LaboratoryId { get; init; }
    public required string LaboratoryName { get; init; }
}

public static class LaboratoryMembershipCollectionExtensions
{
    public static bool HasMember(this IEnumerable<LaboratoryMembershipDto> memberships, Guid userId)
    {
        return memberships.Any(m => m.MemberId == userId);
    }
}