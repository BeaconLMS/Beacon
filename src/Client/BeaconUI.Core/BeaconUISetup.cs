using BeaconUI.Core.Clients;
using BeaconUI.Core.Services;
using Blazored.Modal;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BeaconUI.Core;

public static class BeaconUISetup
{
    public static IServiceCollection AddBeaconUI(this IServiceCollection services, string baseAddress)
    {
        services.AddOptions();
        services.AddAuthorizationCore();
        services.AddScoped<AuthenticationStateProvider, BeaconAuthStateProvider>();
        services.AddScoped(sp => (BeaconAuthStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());

        services.AddHttpClient("BeaconApi", options =>
        {
            options.BaseAddress = new Uri(baseAddress);
        });

        services.AddScoped<AuthClient>();
        services.AddScoped<LabClient>();


        services.AddBlazoredModal();

        return services;
    }
}
