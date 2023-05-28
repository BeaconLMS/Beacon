using Beacon.API.Entities;
using Beacon.Common.Auth;

namespace Beacon.API.Services;

public static class ClaimsPrincipalBuilderExtensions
{
    public static ClaimsPrincipalBuilder ForUser(this ClaimsPrincipalBuilder builder, User user)
    {
        return builder.WithId(user.Id).WithName(user.DisplayName).WithEmail(user.EmailAddress);
    }
}
