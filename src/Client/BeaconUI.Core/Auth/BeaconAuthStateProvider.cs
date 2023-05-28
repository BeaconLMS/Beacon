using Beacon.Common.Auth;
using Microsoft.AspNetCore.Components.Authorization;

namespace BeaconUI.Core.Auth;

public sealed class BeaconAuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;

    public BeaconAuthStateProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await _httpClient.GetCurrentUser();
        return new AuthenticationState(user);
    }

    public void NotifyUserChanged(AuthenticatedUserInfo? user)
    {
        var authState = new AuthenticationState(user);
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    }
}

public static class AuthenticationStateProviderExtensions
{
    public static void NotifyUserChanged(this AuthenticationStateProvider authStateProvider, AuthenticatedUserInfo? user)
    {
        ((BeaconAuthStateProvider)authStateProvider).NotifyUserChanged(user);
    }
}