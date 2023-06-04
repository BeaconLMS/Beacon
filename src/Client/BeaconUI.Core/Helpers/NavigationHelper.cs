using Beacon.Common.Laboratories;

namespace BeaconUI.Core.Helpers;

public static class NavigationHelper
{
    public static string GetLabDetailsHref(Guid labId) => $"laboratories/{labId}";
}
