using Beacon.API.Persistence;
using Beacon.Common.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Beacon.API.Controllers;

[Route("api/[controller]")]
public class UsersController : ApiControllerBase
{
    private readonly BeaconDbContext _context;

    public UsersController(BeaconDbContext context)
    {
        _context = context;
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
    {
        if (!Guid.TryParse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return NotFound();

        var user = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new AuthenticatedUserInfo
            {
                Id = u.Id,
                DisplayName = u.DisplayName,
                EmailAddress = u.EmailAddress,
                Memberships = u.Memberships!.Select(m => new AuthenticatedUserInfo.LabMembershipDto
                {
                    LaboratoryId = m.Laboratory.Id,
                    LaboratoryName = m.Laboratory.Name,
                    LaboratorySlug = m.Laboratory.Slug,
                    MembershipType = m.MembershipType.ToString()
                })
                .ToList()
            })
            .FirstAsync(ct);

        return Ok(user);
    }
}
