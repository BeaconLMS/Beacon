using Beacon.Common;
using Beacon.Common.Auth;
using Beacon.Common.Auth.Requests;
using BeaconUI.Core.Helpers;
using ErrorOr;

namespace BeaconUI.Core.Auth.RequestHandlers;

public class GetCurrentUserRequestHandler : IApiRequestHandler<GetCurrentUserRequest, AuthUserDto>
{
    private readonly HttpClient _httpClient;

    public GetCurrentUserRequestHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ErrorOr<AuthUserDto>> Handle(GetCurrentUserRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync("api/auth/me", cancellationToken);
        return await response.ToErrorOrResult<AuthUserDto>(cancellationToken);
    }
}
