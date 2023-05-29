using Beacon.Common;
using Beacon.Common.Auth;
using Beacon.Common.Auth.Login;
using ErrorOr;
using System.Net.Http.Json;

namespace BeaconUI.Core.Features.Auth;

public class LoginRequestHandler : IApiRequestHandler<LoginRequest, UserDto>
{
    private readonly HttpClient _httpClient;

    public LoginRequestHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ErrorOr<UserDto>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request, cancellationToken);
        return await response.ToErrorOrResult<UserDto>(cancellationToken);
    }
}
