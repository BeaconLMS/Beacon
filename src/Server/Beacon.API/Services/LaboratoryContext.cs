﻿using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Services;

public sealed class LaboratoryContext : ILabContext
{
    private readonly BeaconDbContext _dbContext;

    private readonly Dictionary<Guid, LaboratoryMembershipType?> _membershipTypeCache = new();

    public Guid LaboratoryId { get; }

    public LaboratoryContext(BeaconDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;

        var idValue = httpContextAccessor.HttpContext?.Request.Headers["X-LaboratoryId"];
        LaboratoryId = Guid.TryParse(idValue, out var id) ? id : Guid.Empty;
    }

    public async Task<LaboratoryMembershipType?> GetMembershipTypeAsync(Guid userId, CancellationToken ct = default)
    {
        if (LaboratoryId == Guid.Empty)
            return null;

        if (_membershipTypeCache.TryGetValue(userId, out var type))
            return type;

        var m = await _dbContext.Memberships
            .Where(x => x.LaboratoryId == LaboratoryId && x.MemberId == userId)
            .Select(x => new { x.MembershipType })
            .FirstOrDefaultAsync(ct);

        _membershipTypeCache.Add(userId, m?.MembershipType);
        return m?.MembershipType;
    }
}
