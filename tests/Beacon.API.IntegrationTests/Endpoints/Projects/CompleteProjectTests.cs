﻿using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;

namespace Beacon.API.IntegrationTests.Endpoints.Projects;

[Trait("Feature", "Project Management")]
public sealed class CompleteProjectTests : ProjectTestBase
{
    public CompleteProjectTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[005] Complete project succeeds when request is valid")]
    public async Task CompleteProject_SucceedsWhenRequestIsValid()
    {
        RunAsAdmin();

        var response = await SendAsync(new CompleteProjectRequest
        {
            ProjectId = ProjectId
        });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var project = ExecuteDbContext(db => db.Projects.Single(x => x.Id == ProjectId));
        Assert.Equal(ProjectStatus.Completed, project.ProjectStatus);
    }

    [Fact(DisplayName = "[005] Complete project fails when request is invalid")]
    public async Task CompleteProject_FailsWhenRequestIsInvalid()
    {
        RunAsMember();

        var response = await SendAsync(new CompleteProjectRequest
        {
            ProjectId = ProjectId
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var project = ExecuteDbContext(db => db.Projects.Single(x => x.Id == ProjectId));
        Assert.Equal(ProjectStatus.Active, project.ProjectStatus);
    }
}
