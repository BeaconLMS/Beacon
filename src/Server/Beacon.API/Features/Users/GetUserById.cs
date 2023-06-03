using Beacon.API.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Users;

public static class GetUserById
{
    public sealed record Query(Guid UserId) : IRequest<Response>;
    public sealed record Response(UserDto? User);

    public sealed record UserDto
    {
        public required Guid Id { get; init; }
        public required string DisplayName { get; init; }
        public required string EmailAddress { get; init; }
    }

    public sealed class QueryHandler : IRequestHandler<Query, Response>
    {
        private readonly BeaconDbContext _dbContext;

        public QueryHandler(BeaconDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Where(u => u.Id == request.UserId)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    DisplayName = u.DisplayName,
                    EmailAddress = u.EmailAddress
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            return new Response(user);
        }
    }
}
