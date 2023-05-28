using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Beacon.API.Services;

public interface ICurrentUserService
{
    Guid UserId { get; }
}

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var idClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(idClaim, out var userId))
                throw new InvalidOperationException("User is not logged in.");

            return userId;
        }
    }
}
