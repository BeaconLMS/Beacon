namespace Beacon.API.Entities;

public class Laboratory
{
    public required Guid Id { get; init; }
    public required string Name { get; set; }
    public required string Slug { get; set; }

    public List<LaboratoryMembership> Memberships { get; set; } = new();
}
