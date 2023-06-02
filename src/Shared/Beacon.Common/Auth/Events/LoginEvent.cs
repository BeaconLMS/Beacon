namespace Beacon.Common.Auth.Events;

public sealed record LoginEvent(AuthUserDto LoggedInUser) : IBeaconEvent<LoginEvent>
{
    public static Action<LoginEvent>? OnTrigger { get; set; }

}
