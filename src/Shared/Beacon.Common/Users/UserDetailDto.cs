namespace Beacon.Common.Users;

public record UserDetailDto
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; init; }
    public required string EmailAddress { get; init; }
}
