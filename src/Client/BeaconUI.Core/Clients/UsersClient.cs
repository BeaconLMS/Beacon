using Beacon.Common;
using BeaconUI.Core.Helpers;
using ErrorOr;

namespace BeaconUI.Core.Clients;

public sealed class UsersClient
{
    private readonly HttpClient _httpClient;

    public UsersClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ErrorOr<UserDto>> GetById(Guid userId, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"api/users/{userId}", ct);
        return await response.ToErrorOrResult<UserDto>(ct);
    }

    public async Task<ErrorOr<UserDto>> GetByEmail(string email, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"api/users/{email}", ct);
        return await response.ToErrorOrResult<UserDto>(ct);
    }
}
