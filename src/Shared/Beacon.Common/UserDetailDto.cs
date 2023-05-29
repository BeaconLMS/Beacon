namespace Beacon.Common;

public class UserDetailDto
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; init; }
    public required string EmailAddress { get; init; }
    public required List<LaboratoryMembershipDto> Memberships { get; init; }
}
