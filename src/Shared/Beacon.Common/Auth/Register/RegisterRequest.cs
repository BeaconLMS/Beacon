using MediatR;

namespace Beacon.Common.Auth.Register;

public class RegisterRequest : IBeaconRequest
{
    public string DisplayName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
