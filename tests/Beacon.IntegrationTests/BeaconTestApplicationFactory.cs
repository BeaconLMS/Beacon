using Beacon.API.Persistence;
using Beacon.WebHost;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Data.Common;
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

            // Create open SqliteConnection so EF won't automatically close it.
            services.AddSingleton<DbConnection>(container =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();

                return connection;
            });
            
            services.AddDbContext<BeaconDbContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });
        });

        builder.UseEnvironment("Development");
    }

    public HttpClient CreateClientWithMockAuthentication(Action<BeaconDbContext>? dbAction = null)
    {
        var factory = WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(defaultScheme: "TestScheme")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });
            });
        });

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
            Utilities.EnsureSeeded(db);
            dbAction?.Invoke(db);
        }

        var client = factory.CreateClient();
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
