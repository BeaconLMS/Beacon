using Beacon.API.Persistence;
using Beacon.API.Security;
using Beacon.Common.Auth;
using Beacon.Common.Auth.Login;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Auth.Login;

public class LoginRequestHandler : IRequestHandler<LoginRequest, AuthenticatedUserInfo>
{
    private readonly BeaconDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public LoginRequestHandler(BeaconDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthenticatedUserInfo> Handle(LoginRequest request, CancellationToken ct)
    {
        var user = await _context.Users
            .Where(u => u.EmailAddress == request.EmailAddress)
            .Select(u => new
            {
                Dto = new AuthenticatedUserInfo 
                {
                    Id = u.Id,
                    DisplayName = u.DisplayName,
                    EmailAddress = u.EmailAddress,
                    Memberships = u.Memberships.Select(m => new AuthenticatedUserInfo.LabMembershipDto
                    {
                        LaboratoryId = m.Laboratory.Id,
                        LaboratoryName = m.Laboratory.Name,
                        LaboratorySlug = m.Laboratory.Slug,
                        MembershipType = m.MembershipType.ToString()
                    }).ToList()
                },
                u.HashedPassword,
                u.HashedPasswordSalt
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if (user is null || !_passwordHasher.Verify(request.Password, user.HashedPassword, user.HashedPasswordSalt))
        {
            var failure = new ValidationFailure(nameof(LoginRequest.EmailAddress), "Email address or password was incorrect.");
            throw new ValidationException(new List<ValidationFailure> { failure });
        }

        return user.Dto;
    }
}
