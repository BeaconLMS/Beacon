namespace Beacon.App.Exceptions;

public class AuthenticationRequiredException : BeaconException
{
    public AuthenticationRequiredException() : base(BeaconExceptionType.NotAuthenticated, "User is not logged in.")
    {
        
    }
}
