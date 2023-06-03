using Beacon.Common.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BeaconUI.Core.Shared.Auth;

public sealed class BeaconAuthStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly AuthClient _authClient;

    public ClaimsPrincipal? CurrentUser { get; private set; }

    public BeaconAuthStateProvider(AuthClient authClient)
    {
        _authClient = authClient;
        _authClient.OnLogin += HandleLogin;
        _authClient.OnLogout += HandleLogout;

    }

    public void Dispose()
    {
        _authClient.OnLogin -= HandleLogin;
        _authClient.OnLogout -= HandleLogout;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (CurrentUser == null)
        {
            var result = await _authClient.GetCurrentUserAsync();
            CurrentUser = result.IsError ? AnonymousUser : result.Value.ToClaimsPrincipal();
        }

        return new AuthenticationState(CurrentUser);
    }

    private void HandleLogin(AuthUserDto user)
    {
        UpdateCurrentUser(user);
    }

    private void HandleLogout()
    {
        UpdateCurrentUser(null);
    }

    private void UpdateCurrentUser(AuthUserDto? currentUser)
    {
        CurrentUser = currentUser.ToClaimsPrincipal();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(CurrentUser)));
    }

    private static ClaimsPrincipal AnonymousUser { get; } = new ClaimsPrincipal(new ClaimsIdentity());
}
