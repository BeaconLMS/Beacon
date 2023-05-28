using System.Security.Claims;

namespace Beacon.Common.Auth;

public static class ClaimHelpers
{
    public static bool TryGetUserId(this ClaimsPrincipal cp, out Guid userId)
    {
        var idClaim = cp.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(idClaim, out userId);
    }
}
