using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Requests;

namespace Beacon.IntegrationTests.EndpointTests.Laboratories;

public class CreateLaboratoryTests : IClassFixture<BeaconTestApplicationFactory>
{
    private readonly BeaconTestApplicationFactory _factory;
    private readonly HttpClient _httpClient;

    public CreateLaboratoryTests(BeaconTestApplicationFactory factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClientWithMockAuthentication();
    }

    [Fact]
    public async Task CreateLab_ShouldFail_WhenRequestIsInvalid()
    {
        var response = await _httpClient.PostAsJsonAsync("api/laboratories", new CreateLaboratoryRequest
        {
            LaboratoryName = "no" // must be at least 3 characters
        });

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CreateLab_ShouldSucceed_WhenRequestIsValid()
    {
        var response = await _httpClient.PostAsJsonAsync("api/laboratories", new CreateLaboratoryRequest
        {
            LaboratoryName = "Test Lab"
        });

        response.IsSuccessStatusCode.Should().BeTrue();

        var labSummary = await response.Content.ReadFromJsonAsync<LaboratorySummaryDto>();
        var labDetails = await _httpClient.GetFromJsonAsync<LaboratoryDetailDto>($"api/laboratories/{labSummary?.Id}", BeaconTestApplicationFactory.GetDefaultJsonSerializerOptions());

        (labDetails?.Members).Should().ContainSingle().Which.Id.Should().Be(CurrentUserDefaults.Id);
    }
}
