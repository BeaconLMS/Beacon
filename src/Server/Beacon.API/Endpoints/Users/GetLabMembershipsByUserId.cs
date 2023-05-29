using Beacon.API.Persistence;
using Beacon.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Users;

public class GetLabMembershipsByUserId : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("users/{userId:Guid}/laboratories", async (Guid userId, BeaconDbContext dbContext) =>
        {
            var memberships = await dbContext.LaboratoryMemberships
                .Where(m => m.Member.Id == userId)
                .Select(m => new LaboratoryMembershipDto
                {
                    LaboratoryId = m.Laboratory.Id,
                    LaboratoryName = m.Laboratory.Name,
                    LaboratorySlug = m.Laboratory.Slug,
                    MembershipType = m.MembershipType.ToString()
                })
                .ToListAsync();

            return Results.Ok(memberships);
        });
    }
}
