﻿using Beacon.API.Features.Users;
using Beacon.API.Helpers;
using Beacon.Common.Auth;
using Beacon.Common.Auth.Requests;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Beacon.API.Endpoints;

internal class AuthEndpoints : IApiEndpointGroup
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        var authGroup = app.MapGroup("auth");

        authGroup.MapGet("me", GetCurrentUser);
        authGroup.MapPost("register", Register);
    }

    private static async Task<IResult> GetCurrentUser(ClaimsPrincipal currentUser, ISender sender, CancellationToken ct)
    {
        var userId = currentUser.GetUserId();
        var response = await sender.Send(new GetUserById.Query(userId), ct);

        if (response.User is not { } user)
            return Results.NotFound();

        return Results.Ok(new AuthUserDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            EmailAddress = user.EmailAddress
        });
    }

    private static async Task<IResult> Register(RegisterRequest request, ISender sender, HttpContext httpContext, CancellationToken ct)
    {
        var command = new Register.Command
        {
            DisplayName = request.DisplayName.Trim(),
            EmailAddress = request.EmailAddress.Trim(),
            PlainTextPassword = request.Password
        };

        await sender.Send(command, ct);
        await httpContext.SignInAsync(command.UserId);

        return Results.Created("auth/me", new AuthUserDto
        {
            Id = command.UserId,
            DisplayName = command.DisplayName,
            EmailAddress = command.EmailAddress
        });
    }
}