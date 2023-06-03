using Beacon.API.App.Services;
using Beacon.API.Persistence;
using Beacon.API.Presentation.Endpoints;
using Beacon.API.Presentation.Services;
using Beacon.Common.Auth.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Beacon.API;

public static class BeaconAPI
{ 
    public static IServiceCollection AddBeaconCore(this IServiceCollection services, Action<DbContextOptionsBuilder> dbOptionsAction)
    {
        // Api
        services.AddEndpointsApiExplorer();

        // Auth
        services.AddAuthentication().AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
        services.AddAuthorization();
        services.AddHttpContextAccessor();
        services.AddSingleton<ICurrentUser, CurrentUser>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        // Data
        services.AddDbContext<BeaconDbContext>(dbOptionsAction);
        services.AddScoped<IUnitOfWork, BeaconDbContext>();

        // Framework
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(BeaconAPI).Assembly);
        });

        services.AddValidatorsFromAssemblies(new[]
        {
            typeof(BeaconAPI).Assembly,
            typeof(LoginRequest).Assembly
        });

        return services;
    }

    public static IEndpointRouteBuilder MapBeaconEndpoints(this IEndpointRouteBuilder app)
    {
        // TODO: register via reflection
        AuthEndpoints.Map(app);
        LabEndpoints.Map(app);
        return app;
    }
}
