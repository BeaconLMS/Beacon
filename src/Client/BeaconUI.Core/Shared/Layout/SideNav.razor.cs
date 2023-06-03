using Beacon.Common.Laboratories;
using BeaconUI.Core.Clients;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Shared.Layout;

public sealed partial class SideNav : IDisposable
{
    [Inject] private LabClient LabClient { get; set; } = null!;

    private List<LaboratoryMembershipDto> Memberships { get; set; } = new();

    public void Dispose()
    {
        LabClient.OnCurrentUserMembershipsChanged -= HandleMembershipsChangedEvent;
    }

    protected override async Task OnInitializedAsync()
    {
        LabClient.OnCurrentUserMembershipsChanged += HandleMembershipsChangedEvent;
        await LoadMembershipsAsync();
    }

    private async void HandleMembershipsChangedEvent()
    {
        await LoadMembershipsAsync();
        StateHasChanged();
    }

    private async Task LoadMembershipsAsync()
    {
        var membershipsOrError = await LabClient.GetCurrentUserMembershipsAsync();
        Memberships = membershipsOrError.Match(m => m, _ => new());
    }
}
