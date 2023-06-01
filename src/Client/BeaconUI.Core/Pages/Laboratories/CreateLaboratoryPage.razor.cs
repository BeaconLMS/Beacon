using Beacon.Common.Laboratories.Requests;
using BeaconUI.Core.Shared;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Pages.Laboratories;

public partial class CreateLaboratoryPage
{
    [Inject] private NavigationManager NavManager { get; set; } = null!;
    [Inject] private ISender Sender { get; set; } = null!;

    private CreateLaboratoryRequest Model { get; } = new();

    private async Task Submit(BeaconForm formContext)
    {
        var result = await Sender.Send(Model);

        if (result.IsError)
            formContext.AddErrors(result.Errors);
        else
            NavManager.NavigateTo($"laboratories/{result.Value.Id}");
    }
}