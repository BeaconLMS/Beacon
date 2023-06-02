using Beacon.Common;
using MediatR;

namespace BeaconUI.Core;

public class BeaconNotificationHandler<T> : INotificationHandler<T> where T : IBeaconEvent<T>
{
    public Task Handle(T notification, CancellationToken cancellationToken)
    {
        T.Trigger(notification);
        return Task.CompletedTask;
    }
}