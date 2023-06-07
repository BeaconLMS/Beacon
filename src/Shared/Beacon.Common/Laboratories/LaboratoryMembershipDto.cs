using Beacon.Common.Laboratories.Enums;

namespace Beacon.Common.Laboratories;

public record LaboratoryMembershipDto
{
    public required LaboratoryDto Laboratory { get; init; }
    public required LaboratoryMembershipType MembershipType { get; init; }
}
