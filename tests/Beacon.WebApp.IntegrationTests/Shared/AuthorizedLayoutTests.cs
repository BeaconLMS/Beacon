using BeaconUI.Core.Auth;
using BeaconUI.Core.Shared;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;

namespace Beacon.WebApp.IntegrationTests.Shared;

public class AuthorizedLayoutTests : TestContext
{
    [Fact]
    public void AuthorizedLayout_RedirectsToLogin_WhenUserIsNotAuthorized()
    {
        // Arrange:
        var authContext = this.AddTestAuthorization().SetNotAuthorized();
        var navManager = Services.GetRequiredService<FakeNavigationManager>();

        // Act:
        RenderComponent<AuthorizedLayout>();

        // Assert
        navManager.History.Should().ContainSingle().Which.Uri.Should().Be("login");
    }

    [Fact]
    public void AuthorizedLayout_RedirectsToLogin_WhenLoggedInUserClicksLogout()
    {
        // Arrange
        var mockHttp = Services.AddMockHttpClient();
        mockHttp.When("/api/auth/logout").ThenReturnNoContent();

        var authContext = this.AddTestAuthorization().SetAuthorized("test");
        Services.AddScoped<BeaconAuthClient>();

        var navManager = Services.GetRequiredService<FakeNavigationManager>();

        // Act
        var cut = RenderComponent<AuthorizedLayout>();

        cut.WaitForElement("button#logout").Click();
        cut.WaitForAssertion(() => navManager.History.Last().Uri.Should().Be("login"));
    }
}
