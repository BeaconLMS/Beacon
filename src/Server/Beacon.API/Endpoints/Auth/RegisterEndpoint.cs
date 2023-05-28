using Beacon.API.Entities;
using Beacon.API.Persistence;
using Beacon.API.Security;
using Beacon.API.Services;
using Beacon.Common;
using Beacon.Common.Auth;
using Beacon.Common.Auth.Register;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Beacon.API.Endpoints.Auth;

public class RegisterEndpoint : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("auth/register", async (RegisterRequest request, IMediator mediator) =>
        {
            try
            {
                await mediator.Send(request);
                return Results.NoContent();
            }
            catch (ValidationException ex)
            {
                var errors = new ValidationResult(ex.Errors).ToDictionary();
                return Results.ValidationProblem(errors, statusCode: (int)HttpStatusCode.UnprocessableEntity);
            }
        });
    }
}

public class RegisterRequestHandler : IRequestHandler<RegisterRequest, BeaconResult>
{
    private readonly BeaconDbContext _dbContext;
    private readonly HttpContext _httpContext;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterRequestHandler(BeaconDbContext dbContext, IHttpContextAccessor httpContextAccessor, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _httpContext = httpContextAccessor.HttpContext!;
        _passwordHasher = passwordHasher;
    }

    public async Task<BeaconResult> Handle(RegisterRequest request, CancellationToken ct)
    {
        await EnsureEmailAddressIsUnique(request.EmailAddress, ct);

        var user = new User
        {
            Id = Guid.NewGuid(),
            DisplayName = request.DisplayName,
            EmailAddress = request.EmailAddress,
            HashedPassword = _passwordHasher.Hash(request.Password, out var salt),
            HashedPasswordSalt = salt
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(ct);

        var claimsPrincipal = new ClaimsPrincipalBuilder().ForUser(user).Build("AuthCookie");
        await _httpContext.SignInAsync(claimsPrincipal);

        return BeaconResult.Empty;
    }

    private async Task EnsureEmailAddressIsUnique(string email, CancellationToken ct)
    {
        if (await _dbContext.Users.AnyAsync(u => u.EmailAddress == email, ct) == false)
            return;

        var failure = new ValidationFailure(
            nameof(RegisterRequest.EmailAddress),
            "An account with the specified email address already exists.",
            email);

        throw new ValidationException(new List<ValidationFailure> { failure });
    }
}
