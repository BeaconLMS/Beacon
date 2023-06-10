using Beacon.API.Services;
using Beacon.App.Entities;

namespace Beacon.IntegrationTests;

public static class CurrentUserDefaults
{
    public static Guid Id { get; } = new Guid("3e8d5902-9574-450a-8f23-7243a82795e9");
    public static string DisplayName { get; } = "Test";
    public static string EmailAddress { get; } = "test@test.com";
    public static string Password { get; } = "password123";

    public static User UserEntity { get; } = new User
    {
        Id = Id,
        DisplayName = DisplayName,
        EmailAddress = EmailAddress,
        HashedPassword = new PasswordHasher().Hash(Password, out var salt),
        HashedPasswordSalt = salt
    };
}
