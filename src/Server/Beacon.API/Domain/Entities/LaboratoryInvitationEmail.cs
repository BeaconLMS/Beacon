namespace Beacon.API.Domain.Entities;

public class LaboratoryInvitationEmail
{
    public required Guid Id { get; init; }

    public LaboratoryInvitationEmailDeliveryStatus? DeliveryStatus { get; private set; }

    public required Guid LaboratoryInvitationId { get; init; }
    public LaboratoryInvitation LaboratoryInvitation { get; init; } = null!;

    public void MarkAsSent(string operationId, DateTimeOffset sentOn)
    {
        DeliveryStatus = new LaboratoryInvitationEmailDeliveryStatus
        {
            OperationId = operationId,
            SentOn = sentOn,
            ExpiresOn = sentOn.AddDays(LaboratoryInvitation.ExpireAfterDays)
        };
    }
}
