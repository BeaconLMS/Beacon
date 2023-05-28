using Beacon.Common.Auth;
using System.Net;
using System.Net.Http.Json;

namespace BeaconUI.Core.Auth;

public static class HttpClientExtensions
{
    public static async Task<AuthenticatedUserInfo?> GetCurrentUser(this HttpClient httpClient, CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync("api/users/current", ct);

        if (response.StatusCode is HttpStatusCode.NotFound)
            return null;

        return await response.Content.ReadFromJsonAsync<AuthenticatedUserInfo>(cancellationToken: ct);
    }
}
