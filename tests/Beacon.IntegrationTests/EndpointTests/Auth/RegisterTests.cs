using Beacon.API.Persistence;
using Beacon.Common.Auth.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace Beacon.IntegrationTests.EndpointTests.Auth;

public class RegisterTests : IClassFixture<BeaconTestApplicationFactory>
{
    private readonly BeaconTestApplicationFactory _factory;

    public RegisterTests(BeaconTestApplicationFactory factory)
    {
        _factory = factory;

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
        Utilities.EnsureSeeded(db);
    }

    [Fact]
    public async Task Register_ShouldFail_IfEmailIsMissing()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("api/auth/register", new RegisterRequest
        {
            EmailAddress = "",
            DisplayName = "someValidName",
            Password = "someValidPassword"
        });

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InlineData("nope")]
    [InlineData("nope@")]
    [InlineData("@nope")]
    [InlineData("@nope@")]
    public async Task Register_ShouldFail_IfEmailIsInvalid(string invalidEmail)
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("api/auth/register", new RegisterRequest
        {
            EmailAddress = invalidEmail,
            DisplayName = "someValidName",
            Password = "someValidPassword"
        });

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Register_ShouldFail_IfPasswordIsMissing()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("api/auth/register", new RegisterRequest
        {
            EmailAddress = "someValidEmail@website.com",
            DisplayName = "someValidName",
            Password = ""
        });

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Register_ShouldFail_IfDisplayNameIsMissing()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("api/auth/register", new RegisterRequest
        {
            EmailAddress = "someValidEmail@website.com",
            DisplayName = "",
            Password = "someValidPassword"
        });

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Register_ShouldFail_IfEmailIsTaken()
    {
        

        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("api/auth/register", new RegisterRequest
        {
            EmailAddress = CurrentUserDefaults.EmailAddress,
            DisplayName = "someValidName",
            Password = "someValidPassword"
        });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Register_ShouldSucceed_WhenRequestIsValid()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
            db.Database.EnsureCreated();
        }

        var client = _factory.CreateClient();

        // getting current user should fail if we're not logged in:
        var currentUser = await client.GetAsync("api/auth/me");
        currentUser.IsSuccessStatusCode.Should().BeFalse();

        // register:
        var response = await client.PostAsJsonAsync("api/auth/register", new RegisterRequest
        {
            EmailAddress = "someValidEmail@website.com",
            DisplayName = "someValidName",
            Password = "someValidPassword"
        });

        // check that register was successful:
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // check that auth cookie was included in the response:
        response.Headers.Contains("Set-Cookie");

        // try getting current user again; this time response should be successful:
        currentUser = await client.GetAsync("api/auth/me");
        currentUser.IsSuccessStatusCode.Should().BeTrue();
    }
}
