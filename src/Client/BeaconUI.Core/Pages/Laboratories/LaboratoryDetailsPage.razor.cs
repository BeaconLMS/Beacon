using Beacon.Common.Laboratories;
using BeaconUI.Core.Clients;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Pages.Laboratories;

public partial class LaboratoryDetailsPage
{
    [Inject] private LabClient LabClient { get; set; } = null!;

    [Parameter] public required Guid Id { get; set; }

    private LaboratoryDetailDto? Details { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (Details?.Id != Id)
            await LoadLabDetails();
    }

    private async Task LoadLabDetails()
    {
        var result = await LabClient.GetLaboratoryDetailsAsync(Id);
        Details = result.IsError ? null : result.Value;        
    }
}