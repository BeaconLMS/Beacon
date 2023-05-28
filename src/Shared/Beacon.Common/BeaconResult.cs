using MediatR;

namespace Beacon.Common;

public interface IBeaconRequest : IRequest<BeaconResult> { }
public interface IBeaconRequest<TResponse> : IRequest<BeaconResult<TResponse>> { }

public class BeaconResult
{
    public static BeaconResult Empty => new();
}

public class BeaconResult<T> : BeaconResult
{
    public required T Value { get; set; }
}
