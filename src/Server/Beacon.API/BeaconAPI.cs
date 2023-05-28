using Beacon.API.Behaviors;
using Beacon.API.Endpoints;
using Beacon.API.Persistence;
using Beacon.API.Security;
using Beacon.Common.Auth.Login;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Beacon.API;

public static class BeaconAPI
{
    public static Assembly Assembly { get; } = typeof(BeaconAPI).Assembly;

    public static void AddBeaconCore(this IServiceCollection services, Action<DbContextOptionsBuilder>? optionsAction)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddDbContext<BeaconDbContext>(optionsAction);

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly);
        });

        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
    }

    public static void UseBeaconCore(this RouteGroupBuilder baseGroup)
    {
        var endpoints = Assembly.GetTypes().Where(t => t.GetInterfaces().Any(i => i == typeof(IBeaconEndpoint)));

        foreach (var endpoint in endpoints)
        {
            var mapMethod = endpoint.GetMethod(nameof(IBeaconEndpoint.Map), types: new[] { typeof(IEndpointRouteBuilder) });
            mapMethod!.Invoke(null, new object[] { baseGroup });
        }
    }
}
