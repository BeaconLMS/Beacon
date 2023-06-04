namespace Beacon.API.Domain.Entities;

public class LaboratoryInvitation
{
    public required Guid Id { get; init; }
    public required DateTimeOffset CreatedOn { get; init; }
    public required string SendToEmailAddress { get; init; }

    public required Guid LaboratoryId { get; init; }
    public Laboratory Laboratory { get; init; } = null!;

    public required Guid SentById { get; init; }
    public User SentBy { get; init; } = null!;

    private readonly List<LaboratoryInvitationEmail> _emails = new();
    public IReadOnlyList<LaboratoryInvitationEmail> Emails => _emails;
}
