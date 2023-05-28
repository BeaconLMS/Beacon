﻿using Beacon.Common.Auth.Login;

namespace Beacon.IntegrationTests.EndpointTests.Auth;

public class LoginTests : IClassFixture<BeaconTestApplicationFactory>
{
    private readonly BeaconTestApplicationFactory _factory;
    private readonly HttpClient _httpClient;

    public LoginTests(BeaconTestApplicationFactory factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
    }

    [Fact]
    public async Task Login_ShouldFail_WhenUserDoesNotExist()
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new LoginRequest
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
        await _factory.SeedDbWithUserData("test@test.com", "test", "pwd12345");

        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            EmailAddress = "test@test.com",
            Password = "pwd123456"
        });

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Login_ShouldSucceed_WhenCredentialsAreValid()
    {
        await _factory.SeedDbWithUserData("test@test.com", "test", "pwd12345");

        // getting current user should fail if we're not logged in:
        var currentUser = await _httpClient.GetAsync("api/auth/me");
        currentUser.IsSuccessStatusCode.Should().BeFalse();

        // log in:
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            EmailAddress = "test@test.com",
            Password = "pwd12345"
        });

        // check that login was successful:
        response.IsSuccessStatusCode.Should().BeTrue();

        // check that auth cookie was included in the response:
        response.Headers.Contains("Set-Cookie");

        // try getting current user again; this time response should be successful:
        currentUser = await _httpClient.GetAsync("api/auth/me");
        currentUser.IsSuccessStatusCode.Should().BeTrue();
    }
}
