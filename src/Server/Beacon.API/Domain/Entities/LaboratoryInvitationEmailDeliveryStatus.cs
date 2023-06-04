namespace Beacon.API.Domain.Entities;

public class LaboratoryInvitationEmailDeliveryStatus
{
    public required string OperationId { get; init; }
    public required DateTimeOffset SentOn { get; init; }
    public required DateTimeOffset ExpiresOn { get; init; }
}
