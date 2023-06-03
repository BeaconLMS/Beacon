namespace Beacon.API.Domain.Exceptions;

public sealed class UserNotFoundException : Exception
{
    public UserNotFoundException(Guid memberId) : base($"User with id {memberId} was not found.")
    {
    }
}
