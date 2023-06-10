using Beacon.Common.Auth.Requests;

namespace Beacon.IntegrationTests.EndpointTests.Auth;

public class LogoutTests : EndpointTestBase
{
    public LogoutTests(BeaconTestApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Logout_ShouldSucceed()
    {
        var client = CreateClient();

        // log in:
        await client.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            EmailAddress = CurrentUserDefaults.EmailAddress,
            Password = CurrentUserDefaults.Password
        });

        // current user should be available after logging in:
        var currentUser = await client.GetAsync("api/auth/me");
        currentUser.IsSuccessStatusCode.Should().BeTrue();

        // log out:
        var response = await client.GetAsync("api/auth/logout");

        response.IsSuccessStatusCode.Should().BeTrue();

        // current user should no longer be available after logging out:
        currentUser = await client.GetAsync("api/auth/me");
        currentUser.IsSuccessStatusCode.Should().BeFalse();
    }
}
