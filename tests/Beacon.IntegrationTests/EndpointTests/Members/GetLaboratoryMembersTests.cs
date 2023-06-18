﻿using Beacon.Common.Memberships;

namespace Beacon.IntegrationTests.EndpointTests.Members;

[Collection("MembershipTests")]
public sealed class GetLaboratoryMembersTests : EndpointTestBase
{
    public GetLaboratoryMembersTests(BeaconTestApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetMembers_ReturnsMembers()
    {
        AddTestAuthorization(LaboratoryMembershipType.Admin);
        var members = await CreateClient().GetFromJsonAsync<LaboratoryMemberDto[]>("api/members", JsonSerializerOptions);

        members.Should().ContainSingle().Which.Id.Should().Be(TestData.DefaultUser.Id);

    }
}
