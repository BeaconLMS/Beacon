using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Requests;
using BeaconUI.Core.Helpers;
using ErrorOr;
using System.Net.Http.Json;

namespace BeaconUI.Core.Shared.Laboratories;

public sealed class LabClient
{
    private readonly HttpClient _httpClient;

    public LabClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ErrorOr<LaboratoryDto>> CreateLaboratoryAsync(CreateLaboratoryRequest request, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/laboratories", request, ct);
        return await response.ToErrorOrResult<LaboratoryDto>(ct);
    }
}
