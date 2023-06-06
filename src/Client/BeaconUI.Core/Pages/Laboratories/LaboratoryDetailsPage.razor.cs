using Beacon.Common.Laboratories;
using BeaconUI.Core.Shared.Laboratories;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Pages.Laboratories;

public partial class LaboratoryDetailsPage
{
    [CascadingParameter] public required LaboratoryDetailDto Details { get; set; }

    [CascadingParameter] private IModalService ModalService { get; set; } = null!;

    [Parameter] public required Guid Id { get; set; }

    private async Task ShowInviteMemberModal()
    {
        var modalParameters = new ModalParameters()
            .Add(nameof(InviteMemberForm.LaboratoryId), Id);

        await ModalService.Show<InviteMemberForm>("Invite Lab Member", modalParameters).Result;
    }
}