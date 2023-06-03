using Beacon.Common.Auth.Requests;
using BeaconUI.Core.Shared.Auth;
using BeaconUI.Core.Shared.Common;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Pages.Auth;

public partial class LoginPage
{
    [Inject] private AuthClient AuthClient { get; set; } = null!;
    [Inject] private NavigationManager NavManager { get; set; } = null!;

    private LoginRequest Model { get; set; } = new();

    private async Task Submit(BeaconForm formContext)
    {
        var result = await AuthClient.LoginAsync(Model);
        result.Switch(_ => NavManager.NavigateTo(""), formContext.AddErrors);
    }
}