using Beacon.Common.Auth.Requests;
using BeaconUI.Core.Clients;
using BeaconUI.Core.Shared.Forms;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Pages.Auth;

public partial class RegisterPage
{
    [Inject] private AuthClient AuthClient { get; set; } = null!;
    [Inject] private NavigationManager NavManager { get; set; } = null!;

    private RegisterRequest Model { get; set; } = new();

    private async Task Submit(BeaconForm formContext)
    {
        var result = await AuthClient.RegisterAsync(Model);
        result.Switch(_ => NavManager.NavigateTo(""), formContext.AddErrors);
    }

    private void DoAfterUpdateEmail()
    {
        if (string.IsNullOrWhiteSpace(Model.DisplayName))
            Model.DisplayName = Model.EmailAddress[..Model.EmailAddress.IndexOf('@')];
    }
}