﻿using Beacon.Common.Users;

namespace Beacon.WebApp.IntegrationTests.Auth;

public static class AuthHelper
{
    public static UserDto DefaultUser { get; } = new UserDto
    {
        Id = new Guid("aeaea2c0-ade9-4af9-a0c1-7f49aff0dc54"),
        EmailAddress = "test@test.com",
        DisplayName = "test"
    };
}
