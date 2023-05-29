using Beacon.API.Helpers;
using Beacon.Common.Auth;
using Beacon.Common.Auth.Requests;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Beacon.API.Auth;

[ApiController, Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await _mediator.Send(new GetCurrentUserRequest());
        return result.IsError ? NotFound() : Ok(result.Value);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _mediator.Send(request);
        return await GetLoginResult(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _mediator.Send(request);
        return await GetLoginResult(result);
    }

    [HttpGet("logout")]
    public async Task Logout()
    {
        await HttpContext.SignOutAsync();
    }

    private async Task<IActionResult> GetLoginResult(ErrorOr<UserDto> result)
    {
        if (result.IsError)
            return result.Errors.ToValidationProblemResult();

        var user = result.Value;
        await HttpContext.SignInAsync(user.ToClaimsPrincipal());

        return Ok(user);
    }
}
