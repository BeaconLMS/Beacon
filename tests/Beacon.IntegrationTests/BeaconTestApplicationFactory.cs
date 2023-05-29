using Beacon.API.Entities;
using Beacon.API.Persistence;
using Beacon.API.Security;
using Beacon.Common;
using Beacon.WebHost;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data.Common;
using System.Text.Encodings.Web;

namespace Beacon.IntegrationTests;

public class BeaconTestApplicationFactory : WebApplicationFactory<BeaconWebHost>
{
    public BeaconTestApplicationFactory()
    {
        using var scope = Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<BeaconDbContext>().Database.EnsureCreated();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<BeaconDbContext>>();
            services.RemoveAll<DbConnection>();

            services.AddDbContext<BeaconDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryBeaconDb");
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
            dbContext.Database.EnsureCreated();
        });

        builder.UseEnvironment("Development");
    }

    public Task<User> SeedDbWithDefaultUser() => SeedDbWithUserData("test@test.com", "test", "pwd12345");

    public async Task<User> SeedDbWithUserData(string email, string displayName, string password)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var user = new User
        {
            Id = Guid.NewGuid(),
            EmailAddress = email,
            DisplayName = displayName,
            HashedPassword = passwordHasher.Hash(password, out var salt),
            HashedPasswordSalt = salt
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return user;
    }
}

public static class IWebHostBuilderExtensions
{
    public static IWebHostBuilder SetLoggedInUser(this IWebHostBuilder builder, User? user)
    {
        return builder.ConfigureTestServices(services =>
        {
            services.Configure<TestAuthHandlerOptions>(options => options.LoggedInUser = user == null ? null : new UserDto
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                EmailAddress = user.EmailAddress
            });

            services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                .AddScheme<TestAuthHandlerOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, options => { });
        });
    }
}

public class TestAuthHandlerOptions : AuthenticationSchemeOptions
{
    public UserDto? LoggedInUser { get; set; }
}

public class TestAuthHandler : AuthenticationHandler<TestAuthHandlerOptions>
{
    public const string AuthenticationScheme = "Test";
    public const string UserId = "UserId";

    private readonly UserDto? _loggedInUser;

    public TestAuthHandler(
        IOptionsMonitor<TestAuthHandlerOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
        _loggedInUser = options.CurrentValue.LoggedInUser;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (_loggedInUser is { } user)
        {
            var ticket = new AuthenticationTicket(user, AuthenticationScheme);
            var result = AuthenticateResult.Success(ticket);
            return Task.FromResult(result);
        }
        else
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }
    }
}