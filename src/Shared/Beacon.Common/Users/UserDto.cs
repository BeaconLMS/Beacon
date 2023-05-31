﻿namespace Beacon.Common.Users;

public record UserDto
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; init; }
    public required string EmailAddress { get; init; }
}
