using Beacon.Common.Auth;
using System.Security.Claims;

namespace Beacon.Common.UnitTests.Auth;

public sealed class AuthenticatedUserInfoTests
{
    [Fact]
    public void Mapper_ReturnsAnonymousClaimsPrincipal_WhenUserIsNull()
    {
        AuthenticatedUserInfo? user = null;
        var cp = (ClaimsPrincipal)user;
        cp.Identities.Should().ContainSingle().Which.IsAuthenticated.Should().BeFalse();
    }

    [Fact]
    public void Mapper_ReturnsExpectedClaimsPrincipal_WhenUserIsNotNull()
    {
        var someUser = new AuthenticatedUserInfo
        {
            Id = Guid.NewGuid(),
            EmailAddress = "someone@test.com",
            DisplayName = "Test User",
            Memberships = new()
        };

        var cp = (ClaimsPrincipal)someUser;

        cp.Identities.Should().ContainSingle().Which.IsAuthenticated.Should().BeTrue();
        cp.FindAll(ClaimTypes.NameIdentifier).Should().ContainSingle().Which.Value.Should().Be(someUser.Id.ToString());
        cp.FindAll(ClaimTypes.Name).Should().ContainSingle().Which.Value.Should().Be(someUser.DisplayName);
        cp.FindAll(ClaimTypes.Email).Should().ContainSingle().Which.Value.Should().Be(someUser.EmailAddress);
    }
}
