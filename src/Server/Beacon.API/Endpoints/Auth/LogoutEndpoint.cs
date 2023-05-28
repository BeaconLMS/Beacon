using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Beacon.API.Endpoints.Auth;

public class LogoutEndpoint : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("auth/logout", (HttpContext context) => context.SignOutAsync());
    }
}
