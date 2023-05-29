using Beacon.API.Entities;
using Beacon.API.Persistence;
using Beacon.Common.Laboratories.Create;
using Microsoft.Extensions.DependencyInjection;

namespace Beacon.IntegrationTests.EndpointTests.Laboratories;

public class CreateLaboratoryTests : IClassFixture<BeaconTestApplicationFactory>
{
    private readonly BeaconTestApplicationFactory _factory;
    private readonly HttpClient _httpClient;

    public CreateLaboratoryTests(BeaconTestApplicationFactory factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
    }

    [Fact]
    public async Task Create_ShouldFail_WhenLabSlugIsNotUnique()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
            dbContext.Laboratories.Add(new Laboratory
            {
                Id = Guid.NewGuid(),
                Name = "Some Lab",
                Slug = "some-lab"
            });
            await dbContext.SaveChangesAsync();
        }

        var response = await _httpClient.PostAsJsonAsync("api/laboratories", new CreateLaboratoryRequest
        {
            Name = "My New Lab",
            Slug = "some-lab"
        });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    //[Fact]
    //public async Task Create_ShouldCreateLabWithCurrentUserAsAdmin()
    //{
    //    var user = await _factory.SeedDbWithDefaultUser();
    //    _factory.WithWebHostBuilder(builder => builder.SetLoggedInUser(user));

    //    var response = await _httpClient.PostAsJsonAsync("api/laboratories", new CreateLaboratoryRequest
    //    {
    //        Name = "My New Lab",
    //        Slug = "some-lab"
    //    });

    //    response.StatusCode.Should().Be(HttpStatusCode.OK);
    //}
}
