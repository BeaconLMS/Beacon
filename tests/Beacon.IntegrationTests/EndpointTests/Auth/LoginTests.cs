using Beacon.API.Persistence;
using Beacon.Common.Auth.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace Beacon.IntegrationTests.EndpointTests.Auth;

public class LoginTests : IClassFixture<BeaconTestApplicationFactory>
{
    private readonly BeaconTestApplicationFactory _factory;

    public LoginTests(BeaconTestApplicationFactory factory)
    {
        _factory = factory;

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
        Utilities.EnsureSeeded(db);
    }

    [Fact]
    public async Task Login_ShouldFail_WhenUserDoesNotExist()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            EmailAddress = "nobody@invalid.net",
            Password = "pwd12345"
        });

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Login_ShouldFail_WhenPasswordIsInvalid()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            EmailAddress = CurrentUserDefaults.EmailAddress,
            Password = "not" + CurrentUserDefaults.Password // an invalid password
        });

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Login_ShouldSucceed_WhenCredentialsAreValid()
    {
        var client = _factory.CreateClient();

        // getting current user should fail if we're not logged in:
        var currentUser = await client.GetAsync("api/auth/me");
        currentUser.IsSuccessStatusCode.Should().BeFalse();

        // log in:
        var response = await client.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            EmailAddress = CurrentUserDefaults.EmailAddress,
            Password = CurrentUserDefaults.Password
        });

        // check that login was successful:
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // check that auth cookie was included in the response:
        response.Headers.Contains("Set-Cookie");

        // try getting current user again; this time response should be successful:
        currentUser = await client.GetAsync("api/auth/me");
        currentUser.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
