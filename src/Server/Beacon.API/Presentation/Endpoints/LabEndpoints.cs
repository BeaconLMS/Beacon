using Beacon.API.App.Features.Laboratories;
using Beacon.API.App.Helpers;
using Beacon.Common;
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
        app.MapGet("laboratories/{labId:Guid}", GetDetails);
        app.MapPost("laboratories/{labId:Guid}/invitations", InviteMember);
        app.MapGet("users/me/memberships", GetCurrentUserMemberships);
        app.MapGet("users/{memberId:Guid}/memberships", GetMembershipsByMemberId);
        app.MapGet("invitations/{inviteId:Guid}/accept", AcceptInvitation);
    }

    private static async Task<IResult> Create(CreateLaboratoryRequest request, ISender sender, CancellationToken ct)
    {
        var command = new CreateLaboratory.Command
        {
            LaboratoryName = request.LaboratoryName.Trim()
        };

        await sender.Send(command, ct);

        return Results.Created($"laboratories/{command.LaboratoryId}", new LaboratorySummaryDto
        {
            Id = command.LaboratoryId,
            Name = request.LaboratoryName
        });
    }

    public static async Task<IResult> GetDetails(Guid labId, ISender sender, CancellationToken ct)
    {
        var query = new GetLaboratoryDetails.Query(labId);
        var response = await sender.Send(query, ct);

        return response.Laboratory is not { } lab ? Results.NotFound() : Results.Ok(new LaboratoryDetailDto
        {
            Id = lab.Id,
            Name = lab.Name,
            Members = lab.Members.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Select(u => new UserDto
                {
                    Id = u.Id,
                    DisplayName = u.DisplayName,
                    EmailAddress = u.EmailAddress
                }).ToArray())
        });
    }

    private static async Task<IResult> GetCurrentUserMemberships(ClaimsPrincipal user, ISender sender, CancellationToken ct)
    {
        var currentUserId = user.GetUserId();
        return await GetMembershipsByMemberId(currentUserId, sender, ct);
    }

    private static async Task<IResult> GetMembershipsByMemberId(Guid memberId, ISender sender, CancellationToken ct)
    {
        var query = new GetUserMemberships.Query(memberId);
        var result = await sender.Send(query, ct);

        var memberships = result.Memberships
            .Select(m => new LaboratoryMembershipDto
            {
                Laboratory = new LaboratoryDto
                {
                    Id = m.LaboratoryId,
                    Name = m.LaboratoryName,
                    Admin = new UserDto
                    {
                        Id = m.LaboratoryAdmin.Id,
                        DisplayName = m.LaboratoryAdmin.DisplayName,
                        EmailAddress = m.LaboratoryAdmin.EmailAddress
                    }
                },
                MembershipType = m.MembershipType
            })
            .ToList();

        return Results.Ok(memberships);
    }

    private static async Task<IResult> InviteMember(Guid labId, InviteLabMemberRequest request, ISender sender, CancellationToken ct)
    {
        var command = new InviteNewMember.Command
        {
            NewMemberEmailAddress = request.NewMemberEmailAddress,
            MembershipType = request.MembershipType,
            LaboratoryId = labId
        };

        await sender.Send(command, ct);
        return Results.NoContent();
    }

    private static async Task<IResult> AcceptInvitation(Guid inviteId, Guid emailId, ISender sender, CancellationToken ct)
    {
        var command = new AcceptEmailInvitation.Command(inviteId, emailId);
        await sender.Send(command, ct);
        return Results.NoContent();
    }
}
