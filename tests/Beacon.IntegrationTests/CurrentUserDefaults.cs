namespace Beacon.IntegrationTests;

public static class CurrentUserDefaults
{
    public static Guid Id { get; } = new Guid("3e8d5902-9574-450a-8f23-7243a82795e9");
    public static string DisplayName { get; } = "Test";
    public static string EmailAddress { get; } = "test@test.com";
    public static string Password { get; } = "password123";
}
