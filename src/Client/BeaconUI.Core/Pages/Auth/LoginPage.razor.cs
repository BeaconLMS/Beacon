using Beacon.Common.Auth.Requests;
using BeaconUI.Core.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Pages.Auth;

public partial class LoginPage
{
    [Inject] private NavigationManager NavManager { get; set; } = null!;
    [Inject] private ISender Sender { get; set; } = null!;

    private LoginRequest Model { get; set; } = new();

    private async Task Submit(BeaconForm formContext)
    {
        var result = await Sender.Send(Model);
        result.Switch(_ => NavManager.NavigateTo(""), formContext.AddErrors);
    }
}