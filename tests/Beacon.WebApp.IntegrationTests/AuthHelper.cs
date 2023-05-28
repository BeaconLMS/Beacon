using Beacon.Common.Auth;

namespace Beacon.WebApp.IntegrationTests;

public static class AuthHelper
{
    public static AuthenticatedUserInfo DefaultUser { get; } = new()
    {
        Id = new Guid("aeaea2c0-ade9-4af9-a0c1-7f49aff0dc54"),
        EmailAddress = "test@test.com",
        DisplayName = "test",
        Memberships = new()
    };
}
