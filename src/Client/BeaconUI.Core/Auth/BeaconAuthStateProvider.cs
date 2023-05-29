using Beacon.Common;
using Microsoft.AspNetCore.Components.Authorization;

namespace BeaconUI.Core.Auth;

public sealed class BeaconAuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;

    private UserDto? _currentUser;

    public BeaconAuthStateProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _currentUser ??= await _httpClient.GetCurrentUser();
        return new AuthenticationState(_currentUser);
    }

    public void NotifyUserChanged(UserDto? user)
    {
        var authState = new AuthenticationState(user);
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    }
}

public static class AuthenticationStateProviderExtensions
{
    public static void NotifyUserChanged(this AuthenticationStateProvider authStateProvider, UserDto? user)
    {
        ((BeaconAuthStateProvider)authStateProvider).NotifyUserChanged(user);
    }
}