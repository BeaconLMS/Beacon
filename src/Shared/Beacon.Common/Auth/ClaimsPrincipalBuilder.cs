using System.Security.Claims;

namespace Beacon.Common.Auth;

public class ClaimsPrincipalBuilder
{
    private readonly List<Claim> _claims = new();

    public ClaimsPrincipalBuilder WithId(Guid userId)
    {
        _claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));
        return this;
    }

    public ClaimsPrincipalBuilder WithName(string name)
    {
        _claims.Add(new Claim(ClaimTypes.Name, name));
        return this;
    }

    public ClaimsPrincipalBuilder WithEmail(string email)
    {
        _claims.Add(new Claim(ClaimTypes.Email, email));
        return this;
    }

    public ClaimsPrincipal Build(string? authType = null)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(_claims, authType));
    }
}
