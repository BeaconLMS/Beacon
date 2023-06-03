using Beacon.WebApp.IntegrationTests.Auth;
using BeaconUI.Core;
using BeaconUI.Core.Services;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;

namespace Beacon.WebApp.IntegrationTests;

public class AppTests : TestContext
{
    [Fact]
    public void WebApp_RedirectsToLogin_WhenUserIsNotAuthorized()
    {
        // Arrange
        this.AddTestAuthorization().SetNotAuthorized();
        Services.AddBeaconUI();

        var mockHttp = Services.AddMockHttpClient();
        mockHttp.When(HttpMethod.Get, "/api/auth/me").ThenRespondNotFound();

        var navManager = Services.GetRequiredService<FakeNavigationManager>();

        // Act
        var cut = RenderComponent<BeaconUI.WebApp.App>();
        navManager.NavigateTo("");

        // Assert
        cut.WaitForAssertion(() => navManager.Uri.Should().Be($"{navManager.BaseUri}login"));
    }

    [Fact]
    public async Task WebApp_RedirectsToLogin_WhenLoggedInUserClicksLogout()
    {
        // Arrange
        Services.AddBeaconUI();
        Services.AddScoped<IAuthorizationService, FakeAuthorizationService>();

        var mockHttp = Services.AddMockHttpClient();
        mockHttp.When(HttpMethod.Get, "/api/auth/me").ThenRespondOK(AuthHelper.DefaultUser);
        mockHttp.When(HttpMethod.Get, "/api/auth/logout").ThenRespondNoContent();

        var authProvider = Services.GetRequiredService<BeaconAuthStateProvider>();
        var navManager = Services.GetRequiredService<FakeNavigationManager>();

        // Act
        var cut = RenderComponent<BeaconUI.WebApp.App>();
        navManager.NavigateTo("");

        await cut.WaitForElement("button#logout").ClickAsync(new MouseEventArgs());

        // Assert
        cut.WaitForAssertion(() => navManager.Uri.Should().Be($"{navManager.BaseUri}login"));
    }
}
