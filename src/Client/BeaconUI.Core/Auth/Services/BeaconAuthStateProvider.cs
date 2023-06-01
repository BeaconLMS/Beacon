using Beacon.Common.Auth;
using Beacon.Common.Auth.Requests;
using MediatR;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BeaconUI.Core.Auth.Services;

public sealed class BeaconAuthStateProvider : AuthenticationStateProvider
{
    private readonly ISender _sender;

    private ClaimsPrincipal? CurrentUser { get; set; }

    public BeaconAuthStateProvider(ISender sender)
    {
        _sender = sender;
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

    private static ClaimsPrincipal AnonymousUser { get; } = new ClaimsPrincipal(new ClaimsIdentity());
}
