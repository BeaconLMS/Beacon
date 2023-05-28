﻿using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;

namespace Beacon.WebApp.IntegrationTests.Shared;

public class AuthorizedLayoutTests : TestContext
{
    [Fact]
    public void AuthorizedLayout_RedirectsToLogin_WhenUserIsNotAuthorized()
    {
        // Arrange
        var mockHttp = Services.AddMockHttpClient();
        this.AddAuthServices().SetNotAuthorized();

        var navManager = Services.GetRequiredService<FakeNavigationManager>();

        // Act
        var cut = RenderComponent<BeaconUI.WebApp.App>();
        navManager.NavigateTo("");

        // Assert
        cut.WaitForAssertion(() => navManager.Uri.Should().Be($"{navManager.BaseUri}login"));
    }

    [Fact]
    public void AuthorizedLayout_RedirectsToLogin_WhenLoggedInUserClicksLogout()
    {
        // Arrange
        var mockHttp = Services.AddMockHttpClient();
        mockHttp.When(HttpMethod.Get, "/api/auth/logout").ThenReturnNoContent();

        this.AddAuthServices().SetAuthorized("Test");

        var navManager = Services.GetRequiredService<FakeNavigationManager>();

        // Act
        var cut = RenderComponent<BeaconUI.WebApp.App>();
        navManager.NavigateTo("");

        cut.WaitForElement("button#logout").Click();
        
        // Assert
        cut.WaitForAssertion(() => navManager.Uri.Should().Be($"{navManager.BaseUri}login"));
    }
}
