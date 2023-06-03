using Beacon.API.Features.Laboratories;
using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Requests;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Beacon.API.Endpoints;

internal sealed class LaboratoryEndpoints : IApiEndpointGroup
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        var endpointGroup = app.MapGroup("laboratories");

        endpointGroup.MapPost("", Create);
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
    
}
