using System.Security.Claims;

namespace Beacon.Common.Auth;

public sealed record UserDto
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; init; }
    public required string EmailAddress { get; init; }

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
