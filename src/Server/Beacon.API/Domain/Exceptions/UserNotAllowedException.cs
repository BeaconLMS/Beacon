namespace Beacon.API.Domain.Exceptions;

public class UserNotAllowedException : Exception
{
    public UserNotAllowedException(string message) : base(message)
    {
    }
}
