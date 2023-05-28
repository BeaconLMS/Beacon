using Beacon.Common.Auth;
using BeaconUI.Core.Auth;
using Blazored.LocalStorage;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Beacon.WebApp.IntegrationTests;

public static class AuthHelper
{
    public static TestAuthorizationContext AddAuthServices(this TestContext testContext)
    {
        var authContext = testContext.AddTestAuthorization();

        testContext.Services
            .AddScoped<BeaconAuthService>()
            .AddScoped<AuthenticationStateProvider, BeaconAuthStateProvider>()
            .AddScoped(_ => Mock.Of<ILocalStorageService>());

        return authContext;
    }

    public static UserDetailDto DefaultUser { get; } = new()
    {
        Id = new Guid("aeaea2c0-ade9-4af9-a0c1-7f49aff0dc54"),
        EmailAddress = "test@test.com",
        DisplayName = "test",
        Memberships = new()
    };
}
