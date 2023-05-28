using Beacon.API.Persistence;
using Beacon.API.Security;
using Beacon.API.Services;
using Beacon.Common;
using Beacon.Common.Auth;
using Beacon.Common.Auth.Login;
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

public class LoginEndpoint : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("auth/login", async (LoginRequest request, IMediator mediator) =>
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

public class LoginRequestHandler : IRequestHandler<LoginRequest, BeaconResult>
{
    private readonly BeaconDbContext _dbContext;
    private readonly HttpContext _httpContext;
    private readonly IPasswordHasher _passwordHasher;

    public LoginRequestHandler(BeaconDbContext dbContext, IHttpContextAccessor httpContextAccessor, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _httpContext = httpContextAccessor.HttpContext!;
        _passwordHasher = passwordHasher;
    }

    public async Task<BeaconResult> Handle(LoginRequest request, CancellationToken ct)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress, ct);

        if (user is null || !_passwordHasher.Verify(request.Password, user.HashedPassword, user.HashedPasswordSalt))
        {
            var failure = new ValidationFailure(nameof(LoginRequest.EmailAddress), "Email address or password was incorrect.");
            throw new ValidationException(new List<ValidationFailure> { failure });
        }

        var claimsPrincipal = new ClaimsPrincipalBuilder().ForUser(user).Build("AuthCookie");
        await _httpContext.SignInAsync(claimsPrincipal);

        return BeaconResult.Empty;
    }
}
