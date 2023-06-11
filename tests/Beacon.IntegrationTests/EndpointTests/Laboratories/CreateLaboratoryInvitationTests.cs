using Beacon.Common.Laboratories.Enums;
using Beacon.Common.Laboratories.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beacon.IntegrationTests.EndpointTests.Laboratories;

public class CreateLaboratoryInvitationTests : EndpointTestBase
{
    public CreateLaboratoryInvitationTests(BeaconTestApplicationFactory factory) : base(factory)
    {
        AddTestAuthorization();
    }

    [Fact]
    public async Task InviteMember_CreatesInvitation()
    {
        var client = CreateClient();

        var request = new InviteLabMemberRequest
        {
            MembershipType = LaboratoryMembershipType.Manager,
            NewMemberEmailAddress = "fake@fake.net"
        };

        var response = await client.PostAsJsonAsync($"api/laboratories/{TestData.DefaultLaboratory.Id}/invitations", request);
        response.EnsureSuccessStatusCode();
    }
}
