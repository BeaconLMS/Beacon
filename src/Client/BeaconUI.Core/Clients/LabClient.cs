using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Requests;
using BeaconUI.Core.Helpers;
using ErrorOr;
using System.Net.Http.Json;

namespace BeaconUI.Core.Clients;

public sealed class LabClient
{
    private readonly HttpClient _httpClient;

    public Action? OnCurrentUserMembershipsChanged;

    public LabClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ErrorOr<LaboratorySummaryDto>> CreateLaboratoryAsync(CreateLaboratoryRequest request, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/laboratories", request, ct);
        var result = await response.ToErrorOrResult<LaboratorySummaryDto>(ct);

        if (!result.IsError)
            OnCurrentUserMembershipsChanged?.Invoke();

        return result;
    }

    public async Task<ErrorOr<List<LaboratoryMembershipDto>>> GetCurrentUserMembershipsAsync(CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync("api/users/me/memberships", ct);
        return await response.ToErrorOrResult<List<LaboratoryMembershipDto>>(ct);
    }

    public async Task<ErrorOr<LaboratoryDetailDto>> GetLaboratoryDetailsAsync(Guid id, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"api/laboratories/{id}", ct);
        return await response.ToErrorOrResult<LaboratoryDetailDto>(ct);
    }
}
