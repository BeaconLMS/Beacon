using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Requests;
using BeaconUI.Core.Helpers;
using ErrorOr;
using System.Net.Http.Json;

namespace BeaconUI.Core.Clients;

public sealed class LabClient : ApiClientBase
{
    public Action? OnCurrentUserMembershipsChanged;

    public LabClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<ErrorOr<LaboratorySummaryDto>> CreateLaboratoryAsync(CreateLaboratoryRequest request, CancellationToken ct = default)
    {
        var result = await PostAsync<LaboratorySummaryDto>("api/laboratories", request, ct);

        if (!result.IsError)
            OnCurrentUserMembershipsChanged?.Invoke();

        return result;
    }

    public Task<ErrorOr<List<LaboratoryMembershipDto>>> GetCurrentUserMembershipsAsync(CancellationToken ct = default)
    {
        return GetAsync<List<LaboratoryMembershipDto>>("api/users/me/memberships", ct);
    }

    public async Task<ErrorOr<LaboratoryDetailDto>> GetLaboratoryDetailsAsync(Guid id, CancellationToken ct = default)
    {
        return await GetAsync<LaboratoryDetailDto>($"api/laboratories/{id}", ct);
    }

    public Task<ErrorOr<Success>> InviteMemberAsync(Guid labId, InviteLabMemberRequest request, CancellationToken ct = default)
    {
        return PostAsync($"api/laboratories/{labId}/invitations", request, ct);
    }

    public Task<ErrorOr<Success>> UpdateMembershipType(Guid labId, Guid memberId, UpdateMembershipTypeRequest request, CancellationToken ct = default)
    {
        return PutAsync($"api/laboratories/{labId}/memberships/{memberId}/membershipType", request, ct);
    }
}
