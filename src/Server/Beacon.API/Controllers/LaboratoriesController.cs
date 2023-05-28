using Beacon.API.Persistence;
using Beacon.Common.Laboratories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Controllers;

[Route("api/[controller]")]
public class LaboratoriesController : ApiControllerBase
{
    private readonly BeaconDbContext _context;

    public LaboratoriesController(BeaconDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateNewLaboratoryRequest request, CancellationToken ct)
    {
        var lab = await Mediator.Send(request, ct);
        return Created($"api/laboratories/{lab.Id}", lab);
    }

    [HttpGet("{slugOrId}")]
    public async Task<IActionResult> Get(string slugOrId, CancellationToken ct)
    {
        var query = _context.Laboratories.AsQueryable();

        if (Guid.TryParse(slugOrId, out var labId))
            query = query.Where(x => x.Id == labId);
        else
            query = query.Where(x => x.Slug == slugOrId);

        var lab = await query
            .Select(x => new LaboratorySummaryDto
            {
                Id = x.Id,
                Name = x.Name,
                Slug = x.Slug
            })
            .FirstOrDefaultAsync(ct);

        return lab == null ? NotFound() : Ok(lab);
    }
}
