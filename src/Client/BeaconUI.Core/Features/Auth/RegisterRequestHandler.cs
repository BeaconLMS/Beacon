using Beacon.Common;
using Beacon.Common.Auth;
using Beacon.Common.Auth.Register;
using ErrorOr;
using System.Net.Http.Json;

namespace BeaconUI.Core.Features.Auth;

public class RegisterRequestHandler : IApiRequestHandler<RegisterRequest, UserDto>
{
    private readonly HttpClient _httpClient;

    public RegisterRequestHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ErrorOr<UserDto>> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", request, cancellationToken);
        return await response.ToErrorOrResult<UserDto>(cancellationToken);
    }
}
