using Beacon.Common.Auth.Events;
using BeaconUI.Core.Auth.Services;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Auth.EventHandlers;

internal class LoginEventHandler : INotificationHandler<LoginEvent>
{
    private readonly BeaconAuthStateProvider _authStateProvider;
    private readonly NavigationManager _navManager;

    public LoginEventHandler(BeaconAuthStateProvider authStateProvider, NavigationManager navManager)
    {
        _authStateProvider = authStateProvider;
        _navManager = navManager;
    }

    public Task Handle(LoginEvent notification, CancellationToken cancellationToken)
    {
        _authStateProvider.UpdateCurrentUser(notification.LoggedInUser);
        _navManager.NavigateTo("");

        return Task.CompletedTask;
    }
}
