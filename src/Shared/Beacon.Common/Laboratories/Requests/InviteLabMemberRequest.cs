namespace Beacon.Common.Laboratories.Requests;

public sealed class InviteLabMemberRequest
{
    public required string NewMemberEmailAddress { get; init; }
    public required string MembershipType { get; init; }
}
