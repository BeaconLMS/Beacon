using Bunit.TestDoubles;
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
        mockHttp.When("/api/auth/me").ThenRespondUnauthorized();

        this.AddAuthServices();

        var navManager = Services.GetRequiredService<FakeNavigationManager>();

        // Act
        var cut = RenderComponent<BeaconUI.WebApp.App>();
        navManager.NavigateTo("");

        // Assert
        cut.WaitForAssertion(() => navManager.Uri.Should().Be($"{navManager.BaseUri}login"));
    }
}
