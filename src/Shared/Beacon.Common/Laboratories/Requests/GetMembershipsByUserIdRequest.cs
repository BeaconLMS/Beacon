namespace Beacon.Common.Laboratories.Requests;

public class GetMembershipsByUserIdRequest : IApiRequest<List<LaboratoryMembershipDto>>
{
    public Guid UserId { get; set; }
}
