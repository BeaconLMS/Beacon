namespace Beacon.Common.Users.Requests;

public class SearchUsersRequest : IApiRequest<List<UserDto>>
{
    public string SearchText { get; set; } = string.Empty;

    public List<Guid> ExcludedUserIds { get; set; } = new();
    public List<Guid> ExcludedLaboratoryIds { get; set; } = new();
}
