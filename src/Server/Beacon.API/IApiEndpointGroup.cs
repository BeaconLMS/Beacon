using Microsoft.AspNetCore.Routing;

namespace Beacon.API;

public interface IApiEndpointGroup
{
    static abstract void MapEndpoints(IEndpointRouteBuilder app);
}
