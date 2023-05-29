using Beacon.Common;
using System.Net;
using System.Net.Http.Json;

namespace BeaconUI.Core.Auth;

public static class HttpClientExtensions
{
    public static async Task<UserDto?> GetCurrentUser(this HttpClient httpClient, CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync("api/auth/me", ct);

        if (response.StatusCode is HttpStatusCode.Unauthorized)
            return null;

        return await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken: ct);
    }
}
