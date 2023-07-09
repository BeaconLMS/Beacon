﻿using Beacon.API.Persistence;
using Beacon.App.Services;
using Beacon.Common.Models;
using Beacon.Common.Requests.Laboratories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Laboratories;

public sealed class GetCurrentLaboratory : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("laboratories/current", new GetCurrentLaboratoryRequest()).WithTags(EndpointTags.Laboratories);
    }

    internal sealed class Handler : IRequestHandler<GetCurrentLaboratoryRequest, LaboratoryDto>
    {
        private readonly BeaconDbContext _dbContext;
        private readonly ILabContext _labContext;

        public Handler(BeaconDbContext dbContext, ILabContext labContext)
        {
            _dbContext = dbContext;
            _labContext = labContext;
        }

        public async Task<LaboratoryDto> Handle(GetCurrentLaboratoryRequest request, CancellationToken ct)
        {
            return await _dbContext.Laboratories
                .Where(x => x.Id == _labContext.LaboratoryId)
                .Select(x => new LaboratoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    MemberCount = x.Memberships.Count
                })
                .SingleAsync(ct);
        }
    }
}
