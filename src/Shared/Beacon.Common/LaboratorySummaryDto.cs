namespace Beacon.Common;

public sealed record LaboratorySummaryDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Slug { get; init; }
}
