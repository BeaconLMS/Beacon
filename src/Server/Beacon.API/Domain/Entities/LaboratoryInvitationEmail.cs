namespace Beacon.API.Domain.Entities;

public class LaboratoryInvitationEmail
{
    public required Guid Id { get; init; }
    public required Guid ConfirmationNumber { get; init; }
    public required DateTimeOffset DateSent { get; init; }
    public required DateTimeOffset ExpiresOn { get; init; }
    public required string OperationId { get; init; }

    public LaboratoryInvitationDeliveryStatus Status { get; set; }

    public required Guid InvitationId { get; init; }
    public LaboratoryInvitation Invitation { get; init; } = null!;
}

public enum LaboratoryInvitationDeliveryStatus
{
    Initiated,
    Delivered,
    Failed
}
