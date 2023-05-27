using Beacon.Common.Auth;
using Beacon.Common.Auth.Login;
using Beacon.Common.Responses;
using BeaconUI.Core.Auth;
using BeaconUI.Core.Pages.Auth;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Beacon.WebApp.IntegrationTests.Pages.Auth;

public class LoginPageTests : TestContext
{
    [Fact]
    public void GivenValidCredentials_WhenLoginIsClicked_ThenRedirectToHome()
    {
        // Arrange:
        var mockAuthClient = new Mock<BeaconAuthClient>();
        mockAuthClient
            .Setup(x => x.Login(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserDto
            {
                Id = Guid.NewGuid(),
                EmailAddress = "test@test.com",
                DisplayName = "test"
            });

        Services.AddScoped(_ => mockAuthClient.Object);

        var navManager = Services.GetRequiredService<FakeNavigationManager>();
        var cut = RenderComponent<LoginPage>();

        // Act:
        cut.Find("input[type=email]").Change("test@test.com");
        cut.Find("input[type=password]").Change("password123");
        cut.Find("form").Submit();

        // Assert:
        navManager.History.Should().ContainSingle().Which.Uri.Should().Be("");
    }

    [Fact]
    public void GivenInvalidCredentials_WhenLoginIsClicked_ThenDisplayError()
    {
        // Arrange:
        var mockAuthClient = new Mock<BeaconAuthClient>();
        mockAuthClient
            .Setup(x => x.Login(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationProblemResponse
            {
                Errors = new Dictionary<string, string[]>
                {
                    { nameof(LoginRequest.EmailAddress), new[] { "Some error message" } }
                }
            });

        Services.AddScoped(_ => mockAuthClient.Object);

        var navManager = Services.GetRequiredService<FakeNavigationManager>();
        var cut = RenderComponent<LoginPage>();

        // Act:
        cut.Find("input[type=email]").Change("test@test.com");
        cut.Find("input[type=password]").Change("password123");
        cut.Find("form").Submit();

        navManager.History.Should().BeEmpty();
    }
}