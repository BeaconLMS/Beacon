using Beacon.Common.Auth;
using Beacon.Common.Auth.Login;
using Beacon.Common.Auth.Register;
using Beacon.Common.Responses;
using Blazored.LocalStorage;
using FluentValidation;
using FluentValidation.Results;
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

    public async Task<UserDto?> GetCurrentUserInfo(CancellationToken ct = default)
    {
        return await _localStorage.GetCurrentUserInfo(ct);
    }

    public async Task Login(LoginRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", request, ct);
        await EnsureRequestWasSuccessful(response, ct);
        await ReloadUserDetails();
    }

    public async Task Register(RegisterRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", request, ct);
        await EnsureRequestWasSuccessful(response, ct);
        await ReloadUserDetails();
    }

    public async Task Logout(CancellationToken ct = default)
    {
        var response = await _http.GetAsync("/api/auth/logout", ct);

        response.EnsureSuccessStatusCode();

        await _localStorage.ClearCurrentUserInfo(ct);
        _authStateProvider.NotifyUserChanged(null);
    }

    private async Task ReloadUserDetails()
    {
        if (await _http.GetCurrentUser() is { } user)
        {
            await _localStorage.SetCurrentUserInfo(user);
            _authStateProvider.NotifyUserChanged(user);
        }
    }

    private static async Task EnsureRequestWasSuccessful(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.StatusCode is HttpStatusCode.UnprocessableEntity)
        {
            var problem = await response.Content.ReadFromJsonAsync<ValidationProblemResponse>(cancellationToken: ct);
            var failures = problem?.Errors.SelectMany(e => e.Value.Select(v => new ValidationFailure(e.Key, v)));
            throw new ValidationException(failures);
        }

        response.EnsureSuccessStatusCode();
    }
}
