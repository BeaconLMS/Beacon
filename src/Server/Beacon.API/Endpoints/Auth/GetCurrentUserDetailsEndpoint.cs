using Beacon.API.Persistence;
using Beacon.Common.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Beacon.API.Endpoints.Auth;

public class GetCurrentUserDetailsEndpoint : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("auth/me", async (ClaimsPrincipal claimsPrincipal, BeaconDbContext dbContext) =>
        {
            if (!claimsPrincipal.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var user = await dbContext.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserDetailDto
                {
                    Id = u.Id,
                    DisplayName = u.DisplayName,
                    EmailAddress = u.EmailAddress,
                    Memberships = u.Memberships.Select(m => new LaboratoryMembershipDto
                    {
                        LaboratoryId = m.Laboratory.Id,
                        LaboratoryName = m.Laboratory.Name,
                        LaboratorySlug = m.Laboratory.Slug,
                        MembershipType = m.MembershipType.ToString(),
                    })
                    .ToList()
                })
                .FirstAsync();

            return Results.Ok(user);
        });
    }
}
