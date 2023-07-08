﻿using Beacon.Common.Requests.Laboratories;

namespace Beacon.API.IntegrationTests.Endpoints.Laboratories;

public sealed class CreateLaboratoryTests : TestBase
{
    public CreateLaboratoryTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[002] Create lab succeeds when request is valid")]
    public async Task CreateLab_ShouldSucceed_WhenRequestIsValid()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await PostAsync("api/laboratories", new CreateLaboratoryRequest
        {
            LaboratoryName = "My New Lab"
        });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var createdLab = ExecuteDbContext(db => db.Laboratories.SingleOrDefault(x => x.Name == "My New Lab"));
        Assert.NotNull(createdLab);
    }

    [Fact(DisplayName = "[002] Create lab fails when request is invalid")]
    public async Task CreateLab_ShouldFail_WhenRequestIsInvalid()
    {

        var response = await PostAsync("api/laboratories", new CreateLaboratoryRequest
        {
            LaboratoryName = "no" // must be at least 3 characters
        });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var createdLab = ExecuteDbContext(db => db.Laboratories.SingleOrDefault(x => x.Name == "no"));
        Assert.Null(createdLab);
    }
}
