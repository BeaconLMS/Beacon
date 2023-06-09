﻿using Beacon.API.Behaviors;
using Beacon.API.Endpoints;
using Beacon.API.Middleware;
using Beacon.API.Persistence;
using Beacon.API.Services;
using Beacon.App.Services;
using Beacon.App.Settings;
using Beacon.Common.Requests.Auth;
using Beacon.Common.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

namespace Beacon.API;

public static class BeaconAPI
{
    public static IServiceCollection AddBeaconCore(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(typeof(BeaconAPI).Assembly, typeof(LoginRequest).Assembly);
            config.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });

        services.AddValidatorsFromAssemblies(new[] { typeof(BeaconAPI).Assembly, typeof(LoginRequest).Assembly }, includeInternalTypes: true);
        services.RegisterAuthorizers();
        services.AddScoped<BeaconAuthenticationService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        return services;
    }

    public static IServiceCollection AddBeaconApi(this IServiceCollection services, IConfiguration config, Action<DbContextOptionsBuilder> dbOptionsAction)
    {
        services.AddBeaconCore();

        // Auth
        services.AddAuthentication().AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            };

            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = 403;
                return Task.CompletedTask;
            };
        });
        services.AddAuthorization();

        // Api
        services.AddEndpointsApiExplorer();
        services.Configure<ApplicationSettings>(config.GetRequiredSection("ApplicationSettings"));
        services.ConfigureHttpJsonOptions(options => {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<ISignInManager, SignInManager>();
        services.AddScoped<ILabContext, LaboratoryContext>();

        // Database
        services.AddDbContext<BeaconDbContext>(dbOptionsAction);

        // Email
        services.AddScoped<IEmailService, EmailService>();
        services.Configure<EmailSettings>(config.GetRequiredSection("EmailSettings"));

        return services;
    }

    public static IEndpointRouteBuilder MapBeaconEndpoints<T>(this T app) where T : IApplicationBuilder, IEndpointRouteBuilder
    {
        app.UseExceptionHandler(new ExceptionHandlerOptions
        {
            ExceptionHandler = ExceptionHandler.HandleException
        });

        app.MapGroup("api").MapBeaconEndpoints();

        return app;
    }
}
