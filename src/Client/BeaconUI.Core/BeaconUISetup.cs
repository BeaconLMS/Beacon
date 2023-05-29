using BeaconUI.Core.Auth;
using BeaconUI.Core.Services;
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
        services.AddScoped<BeaconAuthService>();
        services.AddScoped<CurrentUserLabMembershipService>();

        return services;
    }
}
