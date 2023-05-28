using Beacon.API.Entities;
using Beacon.API.Persistence;
using Beacon.Common;
using Beacon.Common.Auth;
using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Create;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace Beacon.API.Endpoints.Laboratories;

public class CreateLaboratoryEndpoint : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("labs", async (CreateLaboratoryRequest request, IMediator mediator) =>
        {
            try
            {
                var lab = await mediator.Send(request);
                return Results.Ok(lab.Value);
            }
            catch (ValidationException ex)
            {
                var errors = new ValidationResult(ex.Errors).ToDictionary();
                return Results.ValidationProblem(errors, statusCode: (int)HttpStatusCode.UnprocessableEntity);
            }
        });
    }
}

public class CreateLaboratoryRequestHandler : IRequestHandler<CreateLaboratoryRequest, BeaconResult<LaboratorySummaryDto>>
{
    private readonly ClaimsPrincipal _currentUser;
    private readonly BeaconDbContext _dbContext;

    public CreateLaboratoryRequestHandler(IHttpContextAccessor httpContextAccessor, BeaconDbContext dbContext)
    {
        _currentUser = httpContextAccessor.HttpContext!.User;
        _dbContext = dbContext;
    }

    public async Task<BeaconResult<LaboratorySummaryDto>> Handle(CreateLaboratoryRequest request, CancellationToken ct)
    {
        await EnsureSlugIsUnique(request.Slug, ct);

        var lab = new Laboratory
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = request.Slug
        };

        _currentUser.TryGetUserId(out var userId);
        var currentUser = await _dbContext.Users.FirstAsync(u => u.Id == userId, ct);
        lab.AddMember(currentUser, LabMembershipType.Admin);

        _dbContext.Laboratories.Add(lab);
        await _dbContext.SaveChangesAsync(ct);

        return new BeaconResult<LaboratorySummaryDto>()
        {
            Value = new LaboratorySummaryDto
            {
                Id = lab.Id,
                Name = lab.Name,
                Slug = lab.Slug
            }
        };
    }

    private async Task EnsureSlugIsUnique(string slug, CancellationToken ct)
    {
        if (await _dbContext.Laboratories.AnyAsync(l => l.Slug == slug, ct) == false)
            return;

        var failure = new ValidationFailure(
            nameof(CreateLaboratoryRequest.Slug),
            "A laboratory with the specified slug already exists.",
            slug);

        throw new ValidationException(new List<ValidationFailure> { failure });
    }
}
