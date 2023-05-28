namespace Beacon.API.Entities;

public class Laboratory
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }

    public List<LabMembership> Members { get; set; } = new();
}
