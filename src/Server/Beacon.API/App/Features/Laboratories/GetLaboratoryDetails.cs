using Beacon.API.App.Services;
using Beacon.API.Domain.Entities;
using Beacon.API.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.App.Features.Laboratories;

public static class GetLaboratoryDetails
{
    public sealed record Query(Guid LaboratoryId) : IRequest<Response>;

    public sealed record Response(LaboratoryDto? Laboratory);

    public sealed record LaboratoryDto
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required Dictionary<string, UserDto[]> Members { get; init; }
    }

    public sealed record UserDto
    {
        public required Guid Id { get; init; }
        public required string DisplayName { get; init; }
        public required string EmailAddress { get; init; }
    }

    public sealed class QueryHandler : IRequestHandler<Query, Response>
    {
        private readonly ICurrentUser _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public QueryHandler(ICurrentUser currentUser, IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var lab = await _unitOfWork
                .GetRepository<Laboratory>()
                .AsQueryable()
                .Where(l => l.Id == request.LaboratoryId)
                .Select(l => new { l.Id, l.Name })
                .FirstOrDefaultAsync(cancellationToken);

            if (lab is null)
                return new Response(null);

            var labMembers = await _unitOfWork
                .GetRepository<LaboratoryMembership>()
                .AsQueryable()
                .Where(m => m.LaboratoryId == lab.Id)
                .Select(m => new
                {
                    MembershipType = m.MembershipType.ToString(),
                    Member = new UserDto
                    {
                        Id = m.Member.Id,
                        DisplayName = m.Member.DisplayName,
                        EmailAddress = m.Member.EmailAddress
                    }
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            if (!labMembers.Any(m => m.Member.Id == _currentUser.UserId))
                throw new LaboratoryMembershipRequiredException(lab.Id);

            return new Response(new LaboratoryDto
            {
                Id = lab.Id,
                Name = lab.Name,
                Members = labMembers
                    .GroupBy(m => m.MembershipType)
                    .ToDictionary(g => g.Key, g => g.Select(m => m.Member).ToArray())
            });
        }
    }
}
