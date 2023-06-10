using Beacon.App.Entities;
using Beacon.App.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Beacon.API.Services;

internal sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork;

    public CurrentUser(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
    {
        _httpContextAccessor = httpContextAccessor;
        _unitOfWork = unitOfWork;
    }

    public Guid UserId
    {
        get
        {
            var idValue = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(idValue, out var id) ? id : Guid.Empty;
        }
    }

    public async Task<User> GetCurrentUserAsync(CancellationToken ct = default)
    {
        var userId = UserId;

        var userQuery = _unitOfWork.QueryFor<User>();

        try
        {
            var user = await userQuery.FirstOrDefaultAsync(u => u.Id == userId, ct);

            return user!;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
       
    }
}
