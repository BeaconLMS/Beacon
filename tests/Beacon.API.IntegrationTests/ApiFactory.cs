﻿using Beacon.API.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Data.Common;
using System.Net.Http.Headers;

namespace Beacon.API.IntegrationTests;

public sealed class ApiFactory : WebApplicationFactory<BeaconWebHost>, IAsyncLifetime
{
    public DbConnection Connection { get; }

    public ApiFactory()
    {
        Connection = new SqliteConnection($"DataSource={Guid.NewGuid()}.db");
    }

    public HttpClient CreateClient(Guid userId, Guid? labId = null)
    {
        var factory = WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services
                    .Configure<TestAuthHandlerOptions>(options => options.UserId = userId)
                    .AddAuthentication(defaultScheme: "TestScheme")
                    .AddScheme<TestAuthHandlerOptions, TestAuthHandler>("TestScheme", options => { });
            });
        });

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "TestScheme");

        if (labId != null)
            client.DefaultRequestHeaders.Add("X-LaboratoryId", labId.Value.ToString());

        return client;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<BeaconDbContext>>();
            services.RemoveAll<BeaconDbContext>();
            services.AddScoped(_ => CreateDbContext());
        });
    }

    async Task IAsyncLifetime.InitializeAsync()
    {
        await Connection.OpenAsync();

        using var dbContext = CreateDbContext();
        await dbContext.Database.EnsureCreatedAsync();
        dbContext.Users.AddRange(TestData.AdminUser, TestData.ManagerUser, TestData.AnalystUser, TestData.MemberUser);
        dbContext.Laboratories.Add(TestData.Lab);
        await dbContext.SaveChangesAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await Connection.CloseAsync();

        using (var dbContext = CreateDbContext())
        {
            await dbContext.Database.EnsureDeletedAsync();
        }

        await Connection.DisposeAsync();
    }

    private BeaconDbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<BeaconDbContext>();
        optionsBuilder.UseSqlite(Connection);
        return new(optionsBuilder.Options);
    }
}
