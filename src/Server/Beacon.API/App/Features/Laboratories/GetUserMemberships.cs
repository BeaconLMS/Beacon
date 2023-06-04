using Beacon.API.App.Services;
using Beacon.API.Domain.Entities;
using Beacon.API.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.App.Features.Laboratories;

public static class GetUserMemberships
{
    public sealed record Query(Guid MemberId) : IRequest<Response>;

    public sealed record Response(List<LaboratoryMembershipDto> Memberships);

    public sealed record LaboratoryMembershipDto
    {
        public required Guid LaboratoryId { get; init; }
        public required string LaboratoryName { get; init; }
        public required UserDto LaboratoryAdmin { get; init; }
        public required LaboratoryMembershipType MembershipType { get; init; }
    }

    public sealed record UserDto
    {
        public required Guid Id { get; init; }
        public required string DisplayName { get; init; }
        public required string EmailAddress { get; init; }
    }

    internal sealed class QueryHandler : IRequestHandler<Query, Response>
    {
        private readonly IUnitOfWork _unitOfWork;

        public QueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var memberships = await _unitOfWork
                .Get<LaboratoryMembership>()
                .AsQueryable()
                .Where(m => m.MemberId == request.MemberId)
                .Select(m => new LaboratoryMembershipDto
                {
                    LaboratoryId = m.LaboratoryId,
                    LaboratoryName = m.Laboratory.Name,
                    LaboratoryAdmin = m.Laboratory.Memberships
                        .Where(lm => lm.MembershipType == LaboratoryMembershipType.Admin)
                        .Select(lm => new UserDto
                        {
                            Id = lm.MemberId,
                            DisplayName = lm.Member.DisplayName,
                            EmailAddress = lm.Member.EmailAddress
                        })
                        .First(),
                    MembershipType = m.MembershipType
                })
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            if (!memberships.Any())
            {
                await VerifyThatUserExists(request.MemberId, cancellationToken);
            }

            return new Response(memberships);
        }

        private async Task VerifyThatUserExists(Guid userId, CancellationToken ct)
        {
            var userExists = await _unitOfWork.Get<User>().AsQueryable().AnyAsync(u => u.Id == userId, ct);

            if (!userExists)
                throw new UserNotFoundException(userId);
        }
    }
}
