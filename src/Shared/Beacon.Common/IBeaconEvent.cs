using MediatR;

namespace Beacon.Common;

public interface IBeaconEvent<T> : INotification where T : IBeaconEvent<T>
{
    static abstract Action<T>? OnTrigger { get; set; }
    
    public static virtual void Trigger(T notification)
    {
        T.OnTrigger?.Invoke(notification);
    }
}
