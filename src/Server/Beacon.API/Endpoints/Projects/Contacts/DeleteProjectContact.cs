﻿using Beacon.API.Persistence;
using Beacon.Common.Requests.Projects.Contacts;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Projects.Contacts;

internal sealed class DeleteProjectContactHandler : IBeaconRequestHandler<DeleteProjectContactRequest>
{
    private readonly BeaconDbContext _dbContext;

    public DeleteProjectContactHandler(BeaconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteProjectContactRequest request, CancellationToken ct)
    {
        await _dbContext.ProjectContacts
            .Where(x => x.Id == request.ContactId && x.ProjectId == request.ProjectId)
            .ExecuteDeleteAsync(ct);

        return Result.Success;
    }
}
