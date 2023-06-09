﻿using Beacon.API.Persistence;
using Beacon.Common.Requests.Auth;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace Beacon.API.IntegrationTests.Endpoints.Auth;

[Collection(nameof(AuthTestFixture))]
public sealed class LogoutTests
{
    private readonly AuthTestFixture _fixture;
    private readonly HttpClient _httpClient;

    public LogoutTests(AuthTestFixture fixture)
    {
        _fixture = fixture;
        _httpClient = fixture.CreateClient();

        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();

        if (db.Database.EnsureCreated())
        {
            db.Users.Add(TestData.AdminUser);
            db.SaveChanges();
        }
    }

    [Fact(DisplayName = "Logged in user can sucessfully log out")]
    public async Task LoggedInUserCanSuccessfullyLogOut()
    {
        await _httpClient.PostAsJsonAsync("api/auth/login", new LoginRequest()
        {
            EmailAddress = TestData.AdminUser.EmailAddress,
            Password = "!!admin"
        });

        await AssertGetCurrentUserStatus(HttpStatusCode.OK);

        await _httpClient.GetAsync("api/auth/logout");

        await AssertGetCurrentUserStatus(HttpStatusCode.Unauthorized);
    }

    private async Task AssertGetCurrentUserStatus(HttpStatusCode expectedStatusCode)
    {
        var response = await _httpClient.GetAsync("api/users/current");
        Assert.Equal(expectedStatusCode, response.StatusCode);
    }

}
