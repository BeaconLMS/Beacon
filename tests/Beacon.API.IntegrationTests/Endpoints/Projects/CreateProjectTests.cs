﻿using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;

namespace Beacon.API.IntegrationTests.Endpoints.Projects;

[Trait("Feature", "Project Management")]
public sealed class CreateProjectTests : ProjectTestBase
{
    private static CreateProjectRequest SomeValidRequest => new()
    {
        CustomerCode = "ABC",
        CustomerName = "ABC Company"
    };

    public static CreateProjectRequest SomeInvalidRequest => new()
    {
        CustomerCode = "ABCD",
        CustomerName = "ABC Company"
    };

    public CreateProjectTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[004] Create project suceeds when request is valid")]
    public async Task CreateProject_Succeeds_WhenRequestIsValid()
    {
        RunAsAdmin();

        var response = await SendAsync(SomeValidRequest);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var createdProject = ExecuteDbContext(db => db.Projects.Single(x => x.CustomerName == SomeValidRequest.CustomerName));
        Assert.Equal($"ABC-{DateTime.Today:yyyyMM}-001", createdProject.ProjectCode.ToString());
        Assert.Equal(TestData.AdminUser.Id, createdProject.CreatedById);
        Assert.Equal(TestData.Lab.Id, createdProject.LaboratoryId);
        Assert.Equal(ProjectStatus.Active, createdProject.ProjectStatus);
    }

    [Fact(DisplayName = "[004] Create project fails when request is invalid")]
    public async Task CreateProject_Fails_WhenRequestIsInvalid()
    {
        RunAsAdmin();

        var response = await SendAsync(SomeInvalidRequest);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var createdProject = ExecuteDbContext(db => db.Projects.SingleOrDefault(x => x.CustomerName == SomeInvalidRequest.CustomerName));
        Assert.Null(createdProject);
    }

    [Fact(DisplayName = "[004] Create project fails when lead analyst is not authorized")]
    public async Task CreateProject_Fails_WhenLeadAnalystIsNotAuthorized()
    {
        RunAsAdmin();

        var response = await SendAsync(new CreateProjectRequest
        {
            CustomerCode = SomeValidRequest.CustomerCode,
            CustomerName = SomeValidRequest.CustomerName,
            LeadAnalystId = TestData.MemberUser.Id
        });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        ExecuteDbContext(db =>
        {
            var projectOrNull = db.Projects.SingleOrDefault(x => x.CustomerName == SomeInvalidRequest.CustomerName);
            Assert.Null(projectOrNull);
        });
    }

    [Fact(DisplayName = "[004] Create project fails when user is not authorized")]
    public async Task CreateProject_Fails_WhenUserIsNotAuthorized()
    {
        RunAsMember();

        var response = await SendAsync(SomeValidRequest);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var createdProject = ExecuteDbContext(db => db.Projects.SingleOrDefault(x => x.CustomerName == SomeValidRequest.CustomerName));
        Assert.Null(createdProject);
    }
}
