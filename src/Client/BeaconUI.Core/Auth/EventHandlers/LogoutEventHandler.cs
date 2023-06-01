using Beacon.Common.Auth.Events;
using BeaconUI.Core.Auth.Services;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BeaconUI.Core.Auth.EventHandlers;

internal sealed record LogoutEventHandler : INotificationHandler<LogoutEvent>
{
    private readonly BeaconAuthStateProvider _authStateProvider;
    private readonly NavigationManager _navManager;

    public LogoutEventHandler(BeaconAuthStateProvider authStateProvider, NavigationManager navManager)
    {
        _authStateProvider = authStateProvider;
        _navManager = navManager;
    }

    public Task Handle(LogoutEvent notification, CancellationToken cancellationToken)
    {
        _authStateProvider.UpdateCurrentUser(null);

        _navManager.NavigateToLogin("login");

        return Task.CompletedTask;
    }
}