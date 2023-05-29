using Beacon.Common;
using Beacon.Common.Auth.Logout;
using ErrorOr;

namespace BeaconUI.Core.Features.Auth;

public class LogoutRequestHandler : IApiRequestHandler<LogoutRequest, Success>
{
    private readonly HttpClient _httpClient;

    public LogoutRequestHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ErrorOr<Success>> Handle(LogoutRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync("api/auth/logout", cancellationToken);
        return response.IsSuccessStatusCode ? Result.Success : Error.Unexpected();
    }
}
