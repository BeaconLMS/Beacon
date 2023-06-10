using Beacon.Common.Auth.Requests;

namespace Beacon.IntegrationTests.EndpointTests.Auth;

public class LoginTests : EndpointTestBase
{
    public LoginTests(BeaconTestApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Login_ShouldFail_WhenUserDoesNotExist()
    {
        var client = CreateClient();

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
        var client = CreateClient();

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
        var client = CreateClient();

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
