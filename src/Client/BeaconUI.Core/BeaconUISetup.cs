﻿using Beacon.Common.Services;
using BeaconUI.Core.Common.Auth;
using BeaconUI.Core.Common.Http;
using Blazored.Modal;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BeaconUI.Core;

public static class BeaconUISetup
{
    public static IServiceCollection AddBeaconUI(this IServiceCollection services, string baseUri)
    {
        services
            .AddSingleton<IApiClient, ApiClient>()
            .AddHttpClient("BeaconApi", o => o.BaseAddress = new Uri(baseUri));

        services
            .AddOptions()
            .AddAuthorizationCore()
            .AddScoped<AuthenticationStateProvider, BeaconAuthStateProvider>()
            .AddScoped<IAuthenticationStateNotifier>(sp => (BeaconAuthStateProvider)sp.GetRequiredService<AuthenticationStateProvider>())
            .AddScoped<ISessionContext>(sp => (BeaconAuthStateProvider)sp.GetRequiredService<AuthenticationStateProvider>())
            .AddScoped<AuthService>()
            .AddBlazoredModal();

        return services;
    }
}
