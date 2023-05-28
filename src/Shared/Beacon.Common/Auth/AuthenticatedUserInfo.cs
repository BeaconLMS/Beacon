using System.Security.Claims;

namespace Beacon.Common.Auth;

public sealed record AuthenticatedUserInfo
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; init; }
    public required string EmailAddress { get; init; }
    public required List<LabMembershipDto> Memberships { get; init; }

    public sealed record LabMembershipDto
    {
        public required Guid LaboratoryId { get; init; }
        public required string LaboratoryName { get; init; }
        public required string LaboratorySlug { get; init; }
        public required string MembershipType { get; init; }
    }

    public static implicit operator ClaimsPrincipal(AuthenticatedUserInfo? user)
    {
        if (user is null)
            return new ClaimsPrincipal(new ClaimsIdentity());

        var identity = new ClaimsIdentity("AuthCookie");

        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        identity.AddClaim(new Claim(ClaimTypes.Name, user.DisplayName));
        identity.AddClaim(new Claim(ClaimTypes.Email, user.EmailAddress));

        return new ClaimsPrincipal(identity);
    }
}
