using Beacon.API.Entities;
using Beacon.API.Persistence;
using Beacon.API.Services;
using Beacon.Common.Laboratories;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Laboratories;

public sealed class CreateLaboratoryRequestHandler : IRequestHandler<CreateNewLaboratoryRequest, LaboratorySummaryDto>
{
    private readonly BeaconDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateLaboratoryRequestHandler(BeaconDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<LaboratorySummaryDto> Handle(CreateNewLaboratoryRequest request, CancellationToken ct)
    {
        await EnsureSlugIsUnique(request.Slug, ct);

        var lab = new Laboratory
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = request.Slug
        };

        var currentUser = await _context.Users.FirstAsync(u => u.Id == _currentUser.UserId, ct);
        lab.AddMember(currentUser, LabMembershipType.Admin);

        _context.Laboratories.Add(lab);
        await _context.SaveChangesAsync(ct);

        return new LaboratorySummaryDto
        {
            Id = lab.Id,
            Name = lab.Name,
            Slug = lab.Slug
        };
    }

    private async Task EnsureSlugIsUnique(string slug, CancellationToken ct)
    {
        if (await _context.Laboratories.AnyAsync(l => l.Slug == slug, ct) == false)
            return;

        var failure = new ValidationFailure(
            nameof(CreateNewLaboratoryRequest.Slug),
            "A laboratory with the specified slug already exists.",
            slug);

        throw new ValidationException(new List<ValidationFailure> { failure });
    }
}
