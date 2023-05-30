namespace Beacon.API.Entities;

public class Laboratory
{
    public required Guid Id { get; init; }
    public required string Name { get; set; }

    private readonly List<LaboratoryMembership> _memberships = new();
    public IReadOnlyList<LaboratoryMembership> Memberships => _memberships;

    private Laboratory() { }

    public static Laboratory CreateNew(string name, User admin)
    {
        var laboratory = new Laboratory
        {
            Id = Guid.NewGuid(),
            Name = name
        };

        laboratory.AddMember(admin, LaboratoryMembershipType.Admin);

        return laboratory;
    }

    public LaboratoryMembership AddMember(User member, LaboratoryMembershipType membershipType)
    {
        var membership = new LaboratoryMembership
        {
            LaboratoryId = Id,
            MemberId = member.Id,
            MembershipType = membershipType
        };

        _memberships.Add(membership);

        return membership;
    }
}