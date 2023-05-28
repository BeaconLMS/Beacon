using Beacon.Common.Auth;
using Beacon.Common.Auth.Login;
using Beacon.Common.Auth.Register;
using Beacon.Common.Responses;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;
using System.Net.Http.Json;

namespace BeaconUI.Core.Auth;

public sealed class BeaconAuthService
{
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;

    public BeaconAuthService(AuthenticationStateProvider authStateProvider, HttpClient http, ILocalStorageService localStorage)
    {
        _authStateProvider = authStateProvider;
        _http = http;
        _localStorage = localStorage;
    }

    public async Task<AuthenticatedUserInfo?> GetCurrentUserInfo(CancellationToken ct = default)
    {
        return await _localStorage.GetCurrentUserInfo(ct);
    }

    public async Task<ValidationProblemResponse?> Login(LoginRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", request, ct);
        return await Login(response, ct);
    }

    public async Task<ValidationProblemResponse?> Register(RegisterRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", request, ct);
        return await Login(response, ct);
    }

    public async Task Logout(CancellationToken ct = default)
    {
        var response = await _http.GetAsync("/api/auth/logout", ct);

        response.EnsureSuccessStatusCode();

        await _localStorage.ClearCurrentUserInfo(ct);
        _authStateProvider.NotifyUserChanged(null);
    }

    private async Task<ValidationProblemResponse?> Login(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.StatusCode is HttpStatusCode.UnprocessableEntity)
        {
            return await GetValidationProblemDetails(response, ct);
        }

        response.EnsureSuccessStatusCode();

        var user = await GetUserDetails(response, ct);

        if (user != null)
        {
            await _localStorage.SetCurrentUserInfo(user, ct);
            _authStateProvider.NotifyUserChanged(user);
        }

        return null;
    }

    private static async Task<AuthenticatedUserInfo?> GetUserDetails(HttpResponseMessage response, CancellationToken ct)
    {
        return await response.Content.ReadFromJsonAsync<AuthenticatedUserInfo>(cancellationToken: ct);
    }

    private static async Task<ValidationProblemResponse?> GetValidationProblemDetails(HttpResponseMessage response, CancellationToken ct)
    {
        return await response.Content.ReadFromJsonAsync<ValidationProblemResponse>(cancellationToken: ct);
    }
}
