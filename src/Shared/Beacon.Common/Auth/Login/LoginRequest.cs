namespace Beacon.Common.Auth.Login;

public class LoginRequest : IBeaconRequest
{
    public string EmailAddress { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
