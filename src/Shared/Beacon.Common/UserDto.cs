using System.Security.Claims;
using Beacon.Common.Auth;

namespace Beacon.Common;

public sealed record UserDto
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; init; }
    public required string EmailAddress { get; init; }

    public static implicit operator UserDto?(ClaimsPrincipal cp)
    {
        if (!cp.TryGetUserId(out var userId))
            return null;

        return new UserDto
        {
            Id = userId,
            DisplayName = cp.Claims.First(c => c.Type == ClaimTypes.Name).Value,
            EmailAddress = cp.Claims.First(c => c.Type == ClaimTypes.Email).Value
        };
    }

    public static implicit operator ClaimsPrincipal(UserDto? user)
    {
        if (user == null)
            return new ClaimsPrincipal(new ClaimsIdentity());

        return new ClaimsPrincipalBuilder()
            .WithId(user.Id)
            .WithName(user.DisplayName)
            .WithEmail(user.EmailAddress)
            .Build("AuthCookie");
    }
}
