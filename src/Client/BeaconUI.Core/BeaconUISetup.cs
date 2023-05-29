﻿using Beacon.Common.Behaviors;
using BeaconUI.Core.Auth;
using MediatR;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BeaconUI.Core;

public static class BeaconUISetup
{
    public static IServiceCollection AddBeaconUI(this IServiceCollection services)
    {
        services.AddOptions();
        services.AddAuthorizationCore();
        services.AddScoped<AuthenticationStateProvider, BeaconAuthStateProvider>();

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

        return services;
    }
}
