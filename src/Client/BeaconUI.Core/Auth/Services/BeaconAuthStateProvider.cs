using Beacon.Common.Auth;
using Beacon.Common.Auth.Events;
using Beacon.Common.Auth.Requests;
using MediatR;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BeaconUI.Core.Auth.Services;

public sealed class BeaconAuthStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly ISender _sender;

    public ClaimsPrincipal? CurrentUser { get; private set; }

    public BeaconAuthStateProvider(ISender sender)
    {
        _sender = sender;

        LoginEvent.OnTrigger += Handle;
        LogoutEvent.OnTrigger += Handle;
    }

    public void Dispose()
    {
        LoginEvent.OnTrigger -= Handle;
        LogoutEvent.OnTrigger -= Handle;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (CurrentUser == null)
        {
            var result = await _sender.Send(new GetCurrentUserRequest());
            CurrentUser = result.IsError ? AnonymousUser : result.Value.ToClaimsPrincipal();
        }

        return new AuthenticationState(CurrentUser);
    }

    public void UpdateCurrentUser(AuthUserDto? currentUser)
    {
        CurrentUser = currentUser.ToClaimsPrincipal();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private void Handle(LoginEvent notification)
    {
        UpdateCurrentUser(notification.LoggedInUser);
    }

    private void Handle(LogoutEvent notification)
    {
        UpdateCurrentUser(null);
    }

    private static ClaimsPrincipal AnonymousUser { get; } = new ClaimsPrincipal(new ClaimsIdentity());
}
