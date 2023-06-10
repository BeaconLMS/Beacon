using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Requests;

namespace Beacon.IntegrationTests.EndpointTests.Laboratories;

public class CreateLaboratoryTests : EndpointTestBase
{
    public CreateLaboratoryTests(BeaconTestApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateLab_ShouldFail_WhenRequestIsInvalid()
    {
        AddTestAuthorization();
        var client = CreateClient();
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
        AddTestAuthorization();
        var client = CreateClient();
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
