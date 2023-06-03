using System.Security.Claims;

namespace Beacon.API.App.Services;

public interface ICurrentUser
{
    Guid UserId { get; }
    ClaimsPrincipal User { get; }
}
