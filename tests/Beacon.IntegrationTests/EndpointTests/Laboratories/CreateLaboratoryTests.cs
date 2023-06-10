using Beacon.API.Persistence;
using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace Beacon.IntegrationTests.EndpointTests.Laboratories;

public class CreateLaboratoryTests : IClassFixture<BeaconTestApplicationFactory>
{
    private readonly BeaconTestApplicationFactory _factory;

    public CreateLaboratoryTests(BeaconTestApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateLab_ShouldFail_WhenRequestIsInvalid()
    {
        var client = _factory.CreateClientWithMockAuthentication();
        var response = await client.PostAsJsonAsync("api/laboratories", new CreateLaboratoryRequest
        {
            LaboratoryName = "no" // must be at least 3 characters
        });

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CreateLab_ShouldSucceed_WhenRequestIsValid()
    {
        var client = _factory.CreateClientWithMockAuthentication();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "TestScheme");

        var response = await client.PostAsJsonAsync("api/laboratories", new CreateLaboratoryRequest
        {
            LaboratoryName = "Test Lab"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var labSummary = await response.Content.ReadFromJsonAsync<LaboratorySummaryDto>();
        var labDetails = await client.GetFromJsonAsync<LaboratoryDetailDto>($"api/laboratories/{labSummary?.Id}", BeaconTestApplicationFactory.GetDefaultJsonSerializerOptions());

        (labDetails?.Members).Should().ContainSingle().Which.Id.Should().Be(CurrentUserDefaults.Id);
    }
}
