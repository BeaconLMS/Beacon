﻿using Beacon.API.IntegrationTests.Collections;
using Beacon.API.Persistence;
using Beacon.App.Entities;

namespace Beacon.API.IntegrationTests.Endpoints.Projects.Contacts;

public sealed class DeleteProjectContactTests : ProjectTestBase
{
    private static Guid ContactId { get; } = Guid.NewGuid();

    public DeleteProjectContactTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "Delete contact succeeds when request is valid")]
    public async Task DeleteContact_Succeeds_WhenRequestIsValid()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await _httpClient.DeleteAsync($"api/projects/{ProjectId}/contacts/{ContactId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.False(ExecuteDbContext(db => db.ProjectContacts.Any(c => c.Id == ContactId)));
    }

    [Fact(DisplayName = "Delete contact fails when user is not authorized")]
    public async Task DeleteContact_FailsWhenUserIsNotAuthorized()
    {
        SetCurrentUser(TestData.MemberUser.Id);

        var response = await _httpClient.DeleteAsync($"api/projects/{ProjectId}/contacts/{ContactId}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.True(ExecuteDbContext(db => db.ProjectContacts.Any(c => c.Id == ContactId)));
    }

    protected override void AddTestData(BeaconDbContext db)
    {
        db.ProjectContacts.Add(new ProjectContact
        {
            Id = ContactId,
            Name = "Johnny Depp",
            EmailAddress = null,
            PhoneNumber = null,
            LaboratoryId = TestData.Lab.Id,
            ProjectId = ProjectId
        });

        base.AddTestData(db);
    }
}
