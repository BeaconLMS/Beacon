using Beacon.Common.Auth.Login;
using Beacon.Common.Auth.Register;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace BeaconUI.Core.Auth;

public sealed class BeaconAuthService
{
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly HttpClient _http;

    public BeaconAuthService(AuthenticationStateProvider authStateProvider, HttpClient http)
    {
        _authStateProvider = authStateProvider;
        _http = http;
    }

    public async Task Login(LoginRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", request, ct);
        await response.ValidateAsync(ct);

        _authStateProvider.NotifyUserChanged(await _http.GetCurrentUser(ct));
    }

    public async Task Register(RegisterRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", request, ct);
        await response.ValidateAsync(ct);

        _authStateProvider.NotifyUserChanged(await _http.GetCurrentUser(ct));
    }

    public async Task Logout(CancellationToken ct = default)
    {
        var response = await _http.GetAsync("/api/auth/logout", ct);
        response.EnsureSuccessStatusCode();

        _authStateProvider.NotifyUserChanged(null);
    }
}
