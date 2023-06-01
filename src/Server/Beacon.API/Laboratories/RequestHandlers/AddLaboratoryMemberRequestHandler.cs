using Beacon.API.Auth.Services;
using Beacon.API.Entities;
using Beacon.API.Persistence;
using Beacon.Common;
using Beacon.Common.Laboratories.Requests;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Laboratories.RequestHandlers;

internal sealed class AddLaboratoryMemberRequestHandler : IApiRequestHandler<AddLaboratoryMemberRequest, Success>
{
    private readonly ICurrentUser _currentUser;
    private readonly BeaconDbContext _dbContext;

    public AddLaboratoryMemberRequestHandler(ICurrentUser currentUser, BeaconDbContext dbContext)
    {
        _currentUser = currentUser;
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Success>> Handle(AddLaboratoryMemberRequest request, CancellationToken ct)
    {
        var result = await LoadData(request, ct);

        return await result.MatchAsync<ErrorOr<Success>>(
            async value =>
            {
                value.Laboratory.AddMember(value.NewMember);
                await _dbContext.SaveChangesAsync(ct);

                return Result.Success;
            },
            async errors => await Task.FromResult(errors));
    }

    private async Task<ErrorOr<(Laboratory Laboratory, User NewMember)>> LoadData(AddLaboratoryMemberRequest request, CancellationToken ct)
    {
        var lab = await _dbContext.Laboratories
            .Where(l => l.Id == request.LaboratoryId && l.Memberships.Any(m => m.MemberId == _currentUser.UserId)) // only search labs that the current user is a member of
            .Include(l => l.Memberships.Where(m => m.Member.EmailAddress == request.NewMemberEmailAddress))
            .AsSplitQuery()
            .FirstOrDefaultAsync(ct);

        if (lab is null)
            return Error.Validation(nameof(AddLaboratoryMemberRequest.LaboratoryId), "The specified laboratory was not found.");

        var newMember = await _dbContext.Users.FirstOrDefaultAsync(u => u.EmailAddress == request.NewMemberEmailAddress, ct);

        if (newMember is null)
            return Error.NotFound(description: "The specified member was not found.");

        if (lab.HasMember(newMember.Id))
            return Error.Validation(nameof(AddLaboratoryMemberRequest.NewMemberEmailAddress), "The specified user is already a member of this lab.");

        return (lab, newMember);
    }
}
