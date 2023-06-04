namespace Beacon.API.Domain.Exceptions;

public sealed class UserNotFoundException : Exception
{
    public UserNotFoundException(Guid userId) : base($"User with id {userId} was not found.")
    {
    }
}
