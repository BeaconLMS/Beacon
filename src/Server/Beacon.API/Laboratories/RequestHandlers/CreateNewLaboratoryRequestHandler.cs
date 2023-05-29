using Beacon.API.Auth.Services;
using Beacon.API.Entities;
using Beacon.API.Persistence;
using Beacon.Common;
using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Requests;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Laboratories.RequestHandlers;

internal sealed class CreateNewLaboratoryRequestHandler : IApiRequestHandler<CreateNewLaboratoryRequest, LaboratoryDto>
{
    private readonly ICurrentUser _currentUser;
    private readonly BeaconDbContext _dbContext;

    public CreateNewLaboratoryRequestHandler(ICurrentUser currentUser, BeaconDbContext dbContext)
    {
        _currentUser = currentUser;
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<LaboratoryDto>> Handle(CreateNewLaboratoryRequest request, CancellationToken ct)
    {
        if (await _dbContext.Laboratories.AnyAsync(x => x.Slug == request.Slug, ct))
            return Error.Validation(nameof(CreateNewLaboratoryRequest.Slug), "A laboratory with the specified slug already exists.");

        var currentUser = await _dbContext.Users.FirstAsync(u => u.Id == _currentUser.UserId, ct);

        var lab = new Laboratory
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = request.Slug
        };

        lab.AddMember(currentUser, LaboratoryMembershipType.Admin);

        _dbContext.Laboratories.Add(lab);
        await _dbContext.SaveChangesAsync(ct);

        return new LaboratoryDto
        {
            Id = lab.Id,
            Name = lab.Name,
            Slug = lab.Slug
        };
    }
}
