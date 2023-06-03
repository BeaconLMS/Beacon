using Beacon.API.App.Features.Laboratories;
using Beacon.API.App.Helpers;
using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Requests;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Beacon.API.Presentation.Endpoints;

internal sealed class LabEndpoints : IApiEndpointMapper
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("laboratories", Create);
        app.MapGet("users/me/memberships", GetCurrentUserMemberships);
        app.MapGet("users/{memberId:Guid}/memberships", GetMembershipsByMemberId);
    }

    private static async Task<IResult> Create(CreateLaboratoryRequest request, ISender sender, CancellationToken ct)
    {
        var command = new CreateLaboratory.Command
        {
            LaboratoryName = request.LaboratoryName.Trim()
        };

        await sender.Send(command, ct);

        return Results.Created($"laboratories/{command.LaboratoryId}", new LaboratoryDto
        {
            Id = command.LaboratoryId,
            Name = request.LaboratoryName
        });
    }

    private static async Task<IResult> GetCurrentUserMemberships(ClaimsPrincipal user, ISender sender, CancellationToken ct)
    {
        var currentUserId = user.GetUserId();
        return await GetMembershipsByMemberId(currentUserId, sender, ct);
    }

    private static async Task<IResult> GetMembershipsByMemberId(Guid memberId, ISender sender, CancellationToken ct)
    {
        var query = new GetLaboratoryMembershipsByMemberId.Query(memberId);
        var result = await sender.Send(query, ct);
        return Results.Ok(result.Memberships);
    }
}
