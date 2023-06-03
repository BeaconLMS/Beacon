using Beacon.Common.Auth;
using Beacon.Common.Auth.Requests;
using BeaconUI.Core.Helpers;
using ErrorOr;
using System.Net.Http.Json;

namespace BeaconUI.Core.Shared.Auth;

public sealed class AuthClient
{
    private readonly HttpClient _httpClient;

    public Action<AuthUserDto>? OnLogin;
    public Action? OnLogout;

    public AuthClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ErrorOr<AuthUserDto>> GetCurrentUserAsync(CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync("api/auth/me", ct);
        return await response.ToErrorOrResult<AuthUserDto>(ct);
    }

    public async Task<ErrorOr<AuthUserDto>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request, ct);
        var result = await response.ToErrorOrResult<AuthUserDto>(ct);

        if (!result.IsError)
            OnLogin?.Invoke(result.Value);

        return result;
    }

    public async Task<ErrorOr<AuthUserDto>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", request, ct);
        var result = await response.ToErrorOrResult<AuthUserDto>(ct);

        if (!result.IsError)
            OnLogin?.Invoke(result.Value);

        return result;
    }

    public async Task<ErrorOr<Success>> LogoutAsync(CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync("api/auth/logout", cancellationToken: ct);

        if (response.IsSuccessStatusCode)
        {
            OnLogout?.Invoke();
            return Result.Success;
        }

        return await response.ToErrorOrResult<Success>(ct);
    }
}
