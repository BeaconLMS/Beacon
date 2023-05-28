using Beacon.Common.Auth;
using System.Net;
using System.Net.Http.Json;

namespace BeaconUI.Core.Auth;

public static class HttpClientExtensions
{
    public static async Task<UserDto?> GetCurrentUser(this HttpClient httpClient, CancellationToken ct = default)
    {
        try
        {
            var response = await httpClient.GetAsync("api/auth/me", ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken: ct);
        }
        catch (HttpRequestException ex) when (ex.StatusCode is HttpStatusCode.Unauthorized)
        {
            return null;
        }
    }
}
