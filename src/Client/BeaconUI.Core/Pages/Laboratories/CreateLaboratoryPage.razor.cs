using Beacon.Common.Laboratories.Requests;
using BeaconUI.Core.Shared.Common;
using BeaconUI.Core.Shared.Laboratories;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Pages.Laboratories;

public partial class CreateLaboratoryPage
{
    [Inject] private LabClient LabClient { get; set; } = null!;
    [Inject] private NavigationManager NavManager { get; set; } = null!;

    private CreateLaboratoryRequest Model { get; } = new();

    private async Task Submit(BeaconForm formContext)
    {
        var result = await LabClient.CreateLaboratoryAsync(Model);

        if (result.IsError)
            formContext.AddErrors(result.Errors);
        else
            NavManager.NavigateTo($"laboratories/{result.Value.Id}");
    }
}