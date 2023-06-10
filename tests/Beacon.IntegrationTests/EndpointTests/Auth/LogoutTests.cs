using Beacon.API.Persistence;
using Beacon.Common.Auth.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace Beacon.IntegrationTests.EndpointTests.Auth;

public class LogoutTests : IClassFixture<BeaconTestApplicationFactory>
{
    private readonly BeaconTestApplicationFactory _factory;

    public LogoutTests(BeaconTestApplicationFactory factory)
    {
        _factory = factory;

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
        Utilities.EnsureSeeded(db);
    }

    [Fact]
    public async Task Logout_ShouldSucceed()
    {
        var client = _factory.CreateClient();

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
