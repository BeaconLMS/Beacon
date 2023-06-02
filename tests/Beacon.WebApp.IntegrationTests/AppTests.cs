using Beacon.Common.Auth.Events;
using Beacon.WebApp.IntegrationTests.Auth;
using BeaconUI.Core;
using BeaconUI.Core.Auth;
using BeaconUI.Core.Auth.Services;
using BeaconUI.Core.Shared;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
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
    public void WebApp_RedirectsToLogin_WhenLoggedInUserClicksLogout()
    {
        // Arrange
        Services.AddBeaconUI();
        Services.AddScoped<IAuthorizationService, FakeAuthorizationService>();

        var mockHttp = Services.AddMockHttpClient();
        mockHttp.When(HttpMethod.Get, "/api/auth/me").ThenRespondOK(AuthHelper.DefaultUser);
        mockHttp.When(HttpMethod.Get, "/api/auth/logout").ThenRespondNoContent();

        var navManager = Services.GetRequiredService<FakeNavigationManager>();

        // Act
        var cut = RenderComponent<BeaconUI.WebApp.App>();
        navManager.NavigateTo("");

        cut.WaitForAssertion(() => cut.FindComponent<MainLayout>());

        var homePage = cut.FindComponent<MainLayout>();
        cut.Find("button#logout").Click();

        // Assert
        homePage.WaitForState(() => navManager.Uri == $"{navManager.BaseUri}login");
        //homePage.WaitForAssertion(() => navManager.Uri.Should().Be($"{navManager.BaseUri}login"), TimeSpan.FromSeconds(10));
    }
}
