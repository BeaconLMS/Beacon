using Beacon.API.Persistence;
using Beacon.Common;
using Beacon.Common.Users;
using Beacon.Common.Users.Requests;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Users.RequestHandlers;

internal class SearchUsersRequestHandler : IApiRequestHandler<SearchUsersRequest, List<UserDto>>
{
    private readonly BeaconDbContext _dbContext;

    public SearchUsersRequestHandler(BeaconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<List<UserDto>>> Handle(SearchUsersRequest request, CancellationToken ct)
    {
        var query = _dbContext.Users.AsQueryable();

        if (request.ExcludedLaboratoryIds is { Count: > 0 } excludedLabIds)
        {
            query = query.Where(u => !u.Memberships.Any(m => excludedLabIds.Contains(m.LaboratoryId)));
        }

        if (request.ExcludedUserIds is { Count: > 0 } excludedUserIds)
        {
            query = query.Where(u => !excludedUserIds.Contains(u.Id));
        }

        // apply search text to display name & email address
        query = query.Where(u => 
            u.EmailAddress.Contains(request.SearchText) || 
            u.DisplayName.Contains(request.SearchText) ||
            request.SearchText.Contains(u.EmailAddress) ||
            request.SearchText.Contains(u.DisplayName));

        return await query
            .Select(u => new UserDto 
            { 
                Id = u.Id, 
                EmailAddress = u.EmailAddress,
                DisplayName = u.DisplayName 
            })
            .AsNoTracking()
            .ToListAsync(ct);
    }
}
