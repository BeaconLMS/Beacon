using Beacon.API.Auth.Services;
using Beacon.API.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Users;

public static class GetUserByCredentials
{
    public sealed record Query(string EmailAddress, string PlainTextPassword) :IRequest<Response>;
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
        private readonly IPasswordHasher _passwordHasher;

        public QueryHandler(BeaconDbContext dbContext, IPasswordHasher passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Where(u => u.EmailAddress == request.EmailAddress)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null || _passwordHasher.Verify(request.PlainTextPassword, user.HashedPassword, user.HashedPasswordSalt))
                return new Response(null);

            return new Response(new UserDto
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                EmailAddress = user.EmailAddress
            });
        }
    }
}
