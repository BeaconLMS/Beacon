﻿using BeaconUI.Core.Clients;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BeaconUI.Core;

public static class BeaconUISetup
{
    public static IServiceCollection AddBeaconUI(this IServiceCollection services)
    {
        services.AddOptions();
        services.AddAuthorizationCore();
        services.AddScoped<AuthenticationStateProvider, BeaconAuthStateProvider>();
        services.AddScoped(sp => (BeaconAuthStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());

        services.AddScoped<AuthClient>();
        services.AddScoped<LabClient>();

        return services;
    }
}
