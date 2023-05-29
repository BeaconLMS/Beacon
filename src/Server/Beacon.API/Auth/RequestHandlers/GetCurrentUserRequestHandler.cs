using Beacon.Common;
using Beacon.Common.Auth;
using Beacon.Common.Auth.Requests;
using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace Beacon.API.Auth.RequestHandlers;

public class GetCurrentUserRequestHandler : IApiRequestHandler<GetCurrentUserRequest, UserDto>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetCurrentUserRequestHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<ErrorOr<UserDto>> Handle(GetCurrentUserRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(GetCurrentUser());
    }

    private ErrorOr<UserDto> GetCurrentUser()
    {
        var user = _httpContextAccessor.HttpContext?.User.ToUserDto();
        return user is null ? Error.NotFound() : user;
    }
}
