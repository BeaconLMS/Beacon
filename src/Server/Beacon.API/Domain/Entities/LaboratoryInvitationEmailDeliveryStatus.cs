namespace Beacon.API.Domain.Entities;

public class LaboratoryInvitationEmailDeliveryStatus
{
    public required Guid Id { get; init; }
    public required string OperationId { get; init; }
    public required DateTimeOffset SentOn { get; init; }
    public required DateTimeOffset ExpiresOn { get; init; }

    public required Guid LaboratoryInvitationEmailId { get; init; }
    public LaboratoryInvitationEmail LaboratoryInvitationEmail { get; init; } = null!;
}
