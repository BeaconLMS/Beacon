using Beacon.Common.Auth.Register;
using BeaconUI.Core.Auth;
using BeaconUI.Core.Shared;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BeaconUI.Core.Pages.Auth;

public partial class RegisterPage
{
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;
    [Inject] private NavigationManager NavManager { get; set; } = null!;
    [Inject] private ISender Sender { get; set; } = null!;

    private RegisterRequest Model { get; set; } = new();

    private async Task Submit(BeaconForm formContext)
    {
        var result = await Sender.Send(Model);

        if (result.IsError)
        {
            formContext.AddErrors(result.Errors);
            return;
        }

        ((BeaconAuthStateProvider)AuthStateProvider).RefreshState();
        NavManager.NavigateTo("");
    }

    private void DoAfterUpdateEmail()
    {
        if (string.IsNullOrWhiteSpace(Model.DisplayName))
            Model.DisplayName = Model.EmailAddress[..Model.EmailAddress.IndexOf('@')];
    }
}