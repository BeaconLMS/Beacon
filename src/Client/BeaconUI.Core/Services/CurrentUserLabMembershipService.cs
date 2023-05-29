using Beacon.Common;
using Beacon.Common.Auth;
using Beacon.Common.Laboratories.Create;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace BeaconUI.Core.Services;

public class CurrentUserLabMembershipService
{
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly HttpClient _httpClient;

    private List<LaboratoryMembershipDto>? _memberships;

    public Action<IReadOnlyList<LaboratoryMembershipDto>>? OnMembershipsChanged;

    public CurrentUserLabMembershipService(AuthenticationStateProvider authStateProvider, HttpClient httpClient)
    {
        _authStateProvider = authStateProvider;
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<LaboratoryMembershipDto>> GetMemberships()
    {
        _memberships ??= await LoadMemberships();
        return _memberships.AsReadOnly();
    }

    public async Task CreateNewLaboratory(CreateLaboratoryRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/laboratories", request);
        await response.ValidateAsync();

        _memberships = await LoadMemberships();
        OnMembershipsChanged?.Invoke(_memberships);
    }

    private async Task<List<LaboratoryMembershipDto>> LoadMemberships()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();

        if (!authState.User.TryGetUserId(out var userId))
            return new List<LaboratoryMembershipDto>();

        return await _httpClient.GetFromJsonAsync<List<LaboratoryMembershipDto>>($"api/users/{userId}/laboratories") ?? new();
    }
}
