using Beacon.API.Persistence;
using Beacon.API.Services;
using Beacon.App.Entities;
using Beacon.WebHost;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Data.Common;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            
            if (dbContext.Database.EnsureCreated())
            {
                dbContext.Users.Add(new User
                {
                    Id = CurrentUserDefaults.Id,
                    DisplayName = CurrentUserDefaults.DisplayName,
                    EmailAddress = CurrentUserDefaults.EmailAddress,
                    HashedPassword = new PasswordHasher().Hash(CurrentUserDefaults.Password, out var salt),
                    HashedPasswordSalt = salt
                });
                dbContext.SaveChanges();
            }           
        });

        builder.UseEnvironment("Development");
    }

    public HttpClient CreateClientWithMockAuthentication()
    {
        var client = WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(defaultScheme: "TestScheme")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });
            });
        })
        .CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "TestScheme");
        return client;
    }

    public static JsonSerializerOptions GetDefaultJsonSerializerOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        options.Converters.Add(new JsonStringEnumConverter());

        return options;
    }
}
