using Beacon.Common.Auth;
using Microsoft.AspNetCore.Components.Authorization;

namespace BeaconUI.Core.Auth;

public sealed class BeaconAuthStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly BeaconAuthClient _authClient;

    public BeaconAuthStateProvider(BeaconAuthClient authClient)
    {
        _authClient = authClient;
        _authClient.OnLogin += HandleLogin;
    }

    public void Dispose()
    {
        _authClient.OnLogin -= HandleLogin;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await _authClient.GetCurrentUser();
        return new AuthenticationState(user);
    }

    private void HandleLogin(AuthenticatedUserInfo user)
    {
        var authState = new AuthenticationState(user);
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    }
}
