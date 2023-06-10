using Beacon.API.Persistence;
using Beacon.WebHost;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace Beacon.IntegrationTests.EndpointTests;

public abstract class EndpointTestBase : IClassFixture<BeaconTestApplicationFactory>
{
    private readonly BeaconTestApplicationFactory _factory;
    private bool _addTestAuthorization;

    public EndpointTestBase(BeaconTestApplicationFactory factory)
    {
        _factory = factory;
    }

    protected void AddTestAuthorization(bool addAuthorization = true)
    {
        _addTestAuthorization = addAuthorization;
    }

    protected IServiceScope CreateScope()
    {
        return _factory.Services.CreateScope();
    }

    protected HttpClient CreateClient()
    {
        var factory = GetWebApplicationFactory();

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
            db.EnsureSeeded();
        }

        var client = factory.CreateClient();

        if (_addTestAuthorization)
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "TestScheme");

        return client;
    }

    private WebApplicationFactory<BeaconWebHost> GetWebApplicationFactory()
    {
        if (!_addTestAuthorization)
        {
            return _factory;
        }

        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(defaultScheme: "TestScheme")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });
            });
        });
    }
}
