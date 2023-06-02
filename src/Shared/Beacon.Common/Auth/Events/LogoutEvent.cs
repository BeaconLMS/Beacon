namespace Beacon.Common.Auth.Events;

public sealed record LogoutEvent : IBeaconEvent<LogoutEvent>
{
    public static Action<LogoutEvent>? OnTrigger { get; set; }
}
